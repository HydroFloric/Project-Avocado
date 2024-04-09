using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Utilities;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum EncryptionType 
{
    DTLS,
    WSS
}

public class MultiplayerNetwork : MonoBehaviour
{
    public static MultiplayerNetwork Instance { get; private set; }

    public string PlayerId { get; private set; }

    public string PlayerName { get; private set; } 


    [SerializeField] string lobbyName = "Lobby";
    [SerializeField] int maxPlayers = 2;
    [SerializeField] EncryptionType encryption = EncryptionType.DTLS;
   
    [SerializeField] SceneReference gameScene;

    
    const string k_keyJoinCode = "RelayJoinCode";
    const string k_dtls = "dtls";
    const string k_wss = "wss";
 

    private Lobby currentLobby;
    private Lobby hostLobby; //experimental stuff
    private string lobbyCode;

    string connectionType => encryption == EncryptionType.DTLS ? k_dtls : k_wss;  //enum to change the encryption type from inspector

    async void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);

        await Authenticate(); //authenticate player as soon as the game starts

    }

    private void Update()
    {/*
      * Both these functions, gooes thru the timer and resets it.
      * if not done then lobby can be deleted after a certain time and the details also might not updates
      */
        HandleLobbyHeartbeat();
        HandleLobbyUpdate();
    }



    async Task Authenticate() //overloaded, because why not?
    {
        await Authenticate("Player" + Random.Range(0, 1000)); //ideally we can set up a login page in which player set up their name and authenicate
    }

    async Task Authenticate(string playerName) //takes the string for player name
    {
        //calls authentication service and sets up a profile for the name. These profile options can be used to store player info like avatar.
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(playerName);

            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        if(!AuthenticationService.Instance.IsSignedIn) //mostly this will be called as we are authenticating player. Unity has anonymous function where players can sign in anonymously
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId; //set player id after signing in 
            PlayerName = playerName; //set player name after signing in
        }
    }

   

    async Task<Allocation> AllocateRelay()
    { // Asynchronously requests an allocation from the Relay Service for setting up relay servers.
        try
        {
            // Creates a relay allocation for the required number of players (excluding the current player).
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation; // Returns the allocation details if successful.
        }
        catch (RelayServiceException ex)
        {
            Debug.Log("Failed to Allocate relay : " + ex.Message);
            return default;
        }
    }

    public async Task CreateLobby()
    {// Asynchronously creates and sets up a new game lobby, including relay allocation and network hosting.
        try
        {
            Allocation allocation = await AllocateRelay(); //Allocates a relay server for the lobby.
            string relayJoinCode = await GetRelayJoinCode(allocation); // Retrieves the join code for the allocated relay.

            // Sets up the lobby options including visibility and initial data.
            CreateLobbyOptions Options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                   {"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member,"false") }
                }
            };

            // Creates the lobby with the specified name, player limit, and options.
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, Options);
            Debug.Log("Created Lobby : " + currentLobby.Name + " with code " + currentLobby.LobbyCode);
            lobbyCode = currentLobby.LobbyCode;
            hostLobby = currentLobby;

            // Updates the lobby with the relay join code for players to connect.
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {k_keyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            // Configures the network manager with the relay server data and starts the host.
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));
            NetworkManager.Singleton.StartHost();

            // Updates the UI to show the lobby and its participants.
            MainMenu.Instance.EnterLobby(lobbyCode);
            PrintPlayers(currentLobby);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log( "Failed to create Lobby : "+ e.Message);
        }
    }


    async Task<string> GetRelayJoinCode(Allocation allocation)
    {// Asynchronously retrieves the join code for the allocated relay server.

        try
        {
            // Requests the join code for the given allocation ID.
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;// Returns the join code if successful.
        }
        catch (RelayServiceException ex)
        {
            Debug.Log("Failed to get Relay Join code : " + ex.Message);
            return default;
        }
    }

    

    async Task<JoinAllocation> JoinRelay(string joinCode)
    {// Attempts to join an existing relay with the provided join code.
        try
        {
            // Requests to join the relay allocation using the join code and waits for the response.
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log("Failed to join relay : " + ex.Message);
            return default;
        }
    }


    public async Task JoinLobbyByCode(string lobbyCode)
    {// Joins an existing lobby using its unique code and sets up networking for game communication.

        try
        {
            // Configures options for joining the lobby, including player information.
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            // Attempts to join the lobby with the provided code and configurations.
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            

            string relayJoinCode = currentLobby.Data[k_keyJoinCode].Value; // Retrieves the relay join code from the lobby's data.
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode); // Uses the relay join code to join the relay, facilitating network communication.

            // Configures the network manager with the obtained relay server data and starts the client.
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, connectionType));
            NetworkManager.Singleton.StartClient();
            Debug.Log("Joined Lobby : " + currentLobby.Name + " with code " + currentLobby.LobbyCode);

            // Updates the UI to reflect the joined lobby and displays the current players.
            MainMenu.Instance.EnterLobby(lobbyCode);
            PrintPlayers(currentLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Failed to quick join lobby : " + e.Message);
        }
    }

    private float heartbeattimer = 15f;
    private async void HandleLobbyHeartbeat()
    { // Sends periodic heartbeat pings to the lobby service if this client is the host.

        if (currentLobby != null && IsHost())
        {
            heartbeattimer -= Time.deltaTime;
            if (heartbeattimer <= 0)
            {
                heartbeattimer = 15f; // Reset timer
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);  // Send heartbeat ping to keep the lobby alive.
            }
        }
    }



    private float lobbyUpdateTimer = 2f;
    private async void HandleLobbyUpdate()
    { // Updates lobby information periodically.

        if (currentLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0)
            {
                lobbyUpdateTimer = 2f; // Reset timer
                try
                {
                    if (IsinLobby())
                    {
                        // Refreshes current lobby data from the service.
                        currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                        // Update UI with the latest lobby details.
                        
                    }
                    MainMenu.Instance.UpdateLobbyDetails(currentLobby);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e); // Handle specific lobby exceptions by nullifying currentLobby.
                    if ((e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound))
                    {
                        currentLobby = null;
                        
                    }
                }
            }
        }

    }


    public async void StartGame() // Initiates the start of the game if the current lobby exists.
    {
        if (currentLobby != null)
        {
            try
            {
                // Prepare lobby update options to mark the game as started.
                UpdateLobbyOptions updateoptions = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                         {"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member,"true") }
                    }
                };

                // Update the lobby data to reflect that the game has started.
                currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateoptions);

                // move from the lobby to the game scene.
                Loader.LoadNetwork(gameScene);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
    }


    public Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        // Constructs and returns a Player object for use in lobby operations.

        Unity.Services.Lobbies.Models.Player player =  new Unity.Services.Lobbies.Models.Player // Create a new Player instance, initializing it with player-specific data
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                // Add player name to the player's data with visibility restricted to lobby members.
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) }
            }
        };
       
        return player; // Return the configured Player object.
    }

    
    public bool IsHost() // Checks if the current player is the host of the lobby.
    {
        if (currentLobby != null && currentLobby.HostId == PlayerId)
        {
            return true;
        }
        else 
        {
            return false; 
        }
    }

    public bool IsinLobby() // Determines if the current player is in the lobby's player list.
    {
        foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players)
        {
            if (player.Id == PlayerId)
            {
                return true;
            }
        }
        currentLobby = null;
        return false;
    }

  
    public bool IsGameStarted() // Checks if the game has been started in the current lobby.
    {
        if (currentLobby != null)
        {
            if (currentLobby.Data["IsGameStarted"].Value == "true")
            {
                return true;
            }
        }
        return false;
    }


    private async void LeaveLobby() //experimental stuff
    {

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    /*
     * Debugging Stuff
     */

    private void PrintPlayers(Lobby lobby) //debugging function for printing players in the lobby
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Debug.Log("name :  " + player.Data["PlayerName"].Value);
        }
    }

    public async Task ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0" , QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }

            };

            QueryResponse queryRespose = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies Found : " + queryRespose.Results.Count);

            foreach (Lobby lobby in queryRespose.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    public async Task QuickJoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();


            string relayJoinCode = currentLobby.Data[k_keyJoinCode].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, connectionType));
            NetworkManager.Singleton.StartClient();
            Debug.Log("Joined Lobby : " + currentLobby.Name + " with code " + currentLobby.LobbyCode);


        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Failed to quick join lobby : " + e.Message);
        }
    }



    public string getLobbyCode()
    {
        return this.lobbyCode;
    }

    public Lobby getCurrentLobby()
    {
        return this.currentLobby;
    }

}

public static class Loader
{
    public static void LoadNetwork(SceneReference gameScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameScene.Name, LoadSceneMode.Single);
    }
}
