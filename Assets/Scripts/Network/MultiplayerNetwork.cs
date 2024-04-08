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

[System.Serializable]
public enum EncryptionType
{
    DTLS,
    WSS
}

public class MultiplayerNetwork : MonoBehaviour
{
    [SerializeField] string lobbyName = "Lobby";
    [SerializeField] int maxPlayers = 2;
    [SerializeField] EncryptionType encryption = EncryptionType.DTLS;
    private Lobby hostLobby;
    private string lobbyCode;
    

    public static MultiplayerNetwork Instance { get; private set; }

    public  string PlayerId {get; private set;}

    public string PlayerName { get; private set;}

    const float k_lobbyHeartbeatInterval = 20f;
    const float k_lobbyPollInterval = 65f;
    const string k_keyJoinCode = "RelayJoinCode";
    const string k_dtls = "dtls";
    const string k_wss = "wss";
 
    CountdownTimer heartbeatTimer = new CountdownTimer(k_lobbyHeartbeatInterval);
    CountdownTimer pollForUpdatesTimer = new CountdownTimer(k_lobbyPollInterval);


    Lobby currentLobby;

    string connectionType => encryption == EncryptionType.DTLS ? k_dtls : k_wss;

    async void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);

        await Authenticate();

        heartbeatTimer.OnTimerStop += () =>
        {
            handleHeartBeatAsync();
            heartbeatTimer.Start();
        };

        pollForUpdatesTimer.OnTimerStop += () =>
        {
            handlePollForUpdatesAsync();
            pollForUpdatesTimer.Start();
        };
    }

   


    async Task Authenticate()
    {
        await Authenticate("Player" + Random.Range(0, 1000));
    }

    async Task Authenticate(string playerName)
    {
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

        if(!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            PlayerName = playerName;
        }
    }

    
    
    public async Task CreateLobby()
    {
        try
        {
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            CreateLobbyOptions Options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, Options);
            Debug.Log("Created Lobby : " + currentLobby.Name + " with code " + currentLobby.LobbyCode);
            lobbyCode = currentLobby.LobbyCode;
            hostLobby = currentLobby;

            //heartbeat timer and poll for updates
            heartbeatTimer.Start();
            pollForUpdatesTimer.Start();

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {k_keyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));
            NetworkManager.Singleton.StartHost();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log( "Failed to create Lobby : "+ e.Message);
        }
    }

    public async Task QuickJoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            pollForUpdatesTimer.Start();

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
    } public async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            pollForUpdatesTimer.Start();

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


    public async Task ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25, Filters = new List<QueryFilter>
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
        catch(LobbyServiceException e)  
        {
            Debug.Log(e);
        }
    }


    async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            return allocation;
        }
        catch(RelayServiceException ex)
        {
            Debug.Log("Failed to Allocate relay : " + ex.Message);
            return default;
        }
    }

    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }catch(RelayServiceException ex)
        {
            Debug.Log("Failed to get Relay Join code : " + ex.Message);
            return default;
        }
    }

    async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch(RelayServiceException ex)
        {
            Debug.Log("Failed to join relay : " + ex.Message);
            return default;
        }
    }

    public Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member) }
                    }
        };
    }


    async Task handleHeartBeatAsync()
    {
        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.Log("Sent heartbeat ping to lobby : " + currentLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to heartbeat lobby : " + e.Message);
        }
    }

    async Task handlePollForUpdatesAsync()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            Debug.Log("polled for updates on lobby  : " + currentLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to poll for updates on lobby : " + e.Message);
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