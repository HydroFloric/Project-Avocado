using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    [SerializeField] SceneReference gameScene;
    private string LobbyCode;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button createBttn;
    [SerializeField] private Button joinBttn;

    [Space(10)]
    [Header("Join Room With Code")]
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button submitName;


    [Space(10)]
    [Header("Ready Panel")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private TMP_Text joinText;
    [SerializeField] private GameObject playerInfoPrefab;
    [SerializeField] private GameObject playerInfoContent;

    [Space(10)]
    [Header("Start Game")]
    [SerializeField] private Button StartGameBttn;

    [Space(10)]
    [Header("Experimental stuff")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private GameObject createPanel;
    public string playerName;


    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        StartGameBttn.gameObject.SetActive(false);
    }

    //set the UI panels coorectly when first booted up
    private void Awake()
    {
        mainPanel.SetActive(true);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

    public void onCreateBttnClick()  //event call for creating lobby
    {
        CreateGame(); 
    }

    async void CreateGame() //call the the function responsible for lobby creation
    {
        await MultiplayerNetwork.Instance.CreateLobby();
    }


    public void EnterLobby(string code)  //Once lobby is successfully creates, enter a room or "party"
    {
        mainPanel.SetActive(false);
        readyPanel.SetActive(true);
        joinText.SetText("Lobby Join Code : " + code);
    }

    public void UpdateLobbyDetails(Lobby currentLobby) //update UI with details like player name that are currently in the lobby
    {
        for (int i = 0; i < playerInfoContent.transform.childCount; i++) //destroy if the panel prefab is already there
        {
            Destroy(playerInfoContent.transform.GetChild(i).gameObject);
        }
        //loops through the players in lobby, get their name and update UI
        foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players) 
        {
            //playerText.SetText(player.Data["PlayerName"].Value);

            GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerInfoContent.transform);
            newPlayerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;
            newPlayerInfo.SetActive(true);
        }

        //only host can start the game
        if (MultiplayerNetwork.Instance.IsHost()) //calls multiplayer script instance and checks if the current player is host or not
        {
            //if true, then start game buttom is activated only on host side.
            StartGameBttn.gameObject.SetActive(true);
        }
        else
        {
            StartGameBttn.gameObject.SetActive(false);
        }

        //if game is started then turn off the UI panels
        if (MultiplayerNetwork.Instance.IsGameStarted() == true) //checks for both host and client as well
        {
            setUIOff();
        }

    }
   

    public void onJoinBttnClick() //update the UI when join button is clicked
    {
        joinPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    public void onSubmitClick() //part of the join panel where it take the user input and pass it to lobby code string
    {
        if(joinPanel.activeSelf)
        {
            LobbyCode = joinCodeInput.text;
            Debug.Log(LobbyCode);
            //once done, player will be able to join the lobby using the code
            JoinGameByCode();
        }
    }

    async void JoinGameByCode() //call the the function responsible for joining lobby
    {
        await MultiplayerNetwork.Instance.JoinLobbyByCode(LobbyCode);
    }


    public void onStartGameBttn() //only host can call for this action. Also, only host can see the start button. 
    {
        if(MultiplayerNetwork.Instance.IsHost())
        {
            MultiplayerNetwork.Instance.StartGame(); 
        }
    }

    public void setUIOff() //sets all of the UI off
    {
        mainPanel.SetActive(false);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }
   

    async void JoinGame() //quick join for testing
    {
        await MultiplayerNetwork.Instance.QuickJoinLobby();
    }

    

    async void ListGame() //list lobbies for testing
    {
        await MultiplayerNetwork.Instance.ListLobbies();
    }


    /*
     * All the experimental stuff
     */
    public void leaveRoom() //experimental stuff
    {
        mainPanel.SetActive(true);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

   

}


