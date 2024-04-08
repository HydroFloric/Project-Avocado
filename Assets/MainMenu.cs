using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] SceneReference gameScene;

    [SerializeField] private Button createBttn;
   [SerializeField] private Button joinBttn;
   [SerializeField] private Button submitName;
    [SerializeField] private TMP_Text joinText;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField joinCodeInput;


   [SerializeField] private GameObject mainPanel;
   [SerializeField] private GameObject createPanel;
   [SerializeField] private GameObject joinPanel;
   [SerializeField] private GameObject readyPanel;

    

    public string playerName;
    public string LobbyCode;
    

    private void Awake()
    {
        mainPanel.SetActive(true);
        createPanel.SetActive(false);
        joinPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

    public void onCreateBttnClick()
    {
        //createPanel.SetActive(true);
        CreateGame();
        mainPanel.SetActive(false);
        readyPanel.SetActive(true);
        
        LobbyCode = MultiplayerNetwork.Instance.getLobbyCode();
        joinText.SetText("Lobby Join Code : " + LobbyCode);
        Debug.Log(LobbyCode);
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

}

public static class Loader
{
    public static void LoadNetwork(SceneReference gameScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameScene.Name, LoadSceneMode.Single);
    }
}
