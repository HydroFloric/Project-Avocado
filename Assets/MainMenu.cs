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

    [SerializeField] private Button createBttn;
   [SerializeField] private Button joinBttn;
   [SerializeField] private Button submitName;
   [SerializeField] private Button StartGame;
    [SerializeField] private TMP_Text joinText;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField joinCodeInput;


   [SerializeField] private GameObject mainPanel;
   [SerializeField] private GameObject createPanel;
   [SerializeField] private GameObject joinPanel;
   [SerializeField] private GameObject readyPanel;

    [SerializeField] private GameObject playerInfoPrefab;
    [SerializeField] private GameObject playerInfoContent;
    [SerializeField] private TMP_Text playerText;

    

    public string playerName;
    public string LobbyCode;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    private void Awake()
    {
        mainPanel.SetActive(true);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

    public void onCreateBttnClick()  //event call for creating lobby
    {
        //createPanel.SetActive(true);
        CreateGame();
        
    }

    public void EnterLobby(string code)  //UI update after lobby started
    {
        
        mainPanel.SetActive(false);
        readyPanel.SetActive(true);
        joinText.SetText("Lobby Join Code : " + code);
    }

    public void UpdateLobbyDetails(Lobby currentLobby)
    {
        for (int i = 0; i < playerInfoContent.transform.childCount; i++)
        {
            Destroy(playerInfoContent.transform.GetChild(i).gameObject);
        }
        foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players)
        {
            //playerText.SetText(player.Data["PlayerName"].Value);
           

            GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerInfoContent.transform);
            newPlayerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;
        }
    }

    public void leaveRoom()
    {
        mainPanel.SetActive(true);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

    public void onJoinBttnClick()
    {
        joinPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    public void onSubmitClick()
    {
        playerName = nameInput.text;
        Debug.Log(playerName);

        if(joinPanel.activeSelf)
        {
            LobbyCode = joinCodeInput.text;
            Debug.Log(LobbyCode);
            JoinGameByCode();
        }
        //transition to lobby ready scene
    }
    public void onStartGameBttn()
    {
        
        
            Loader.LoadNetwork(gameScene);
        
       
    }
    async void CreateGame()
    {
        await MultiplayerNetwork.Instance.CreateLobby();

        //Loader.LoadNetwork(gameScene);
    }

    async void JoinGame()
    {
        await MultiplayerNetwork.Instance.QuickJoinLobby();
    }

    async void JoinGameByCode()
    {
        await MultiplayerNetwork.Instance.JoinLobbyByCode(LobbyCode);
    }

    async void ListGame()
    {
        await MultiplayerNetwork.Instance.ListLobbies();
    }

    public string GetCode()
    {
        return this.LobbyCode;
    }
}

public static class Loader
{
    public static void LoadNetwork(SceneReference gameScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameScene.Name, LoadSceneMode.Single);
    }
}
