using Eflatun.SceneReference;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvent : MonoBehaviour
{
    private UIDocument document;
    private Button createBttn;
    private Button joinBttn;
    private Button listBttn;
    private Button publicBttn;

    private VisualElement mainContainer;
    private VisualElement createContainer;

    [SerializeField] SceneReference gameScene;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        createBttn = document.rootVisualElement.Q("createLobbyBttn") as Button;

        joinBttn = document.rootVisualElement.Q("joinLobbyBttn") as Button;

        listBttn = document.rootVisualElement.Q("listLobbyBttn") as Button;

        publicBttn = document.rootVisualElement.Q("public") as Button;
        createContainer = document.rootVisualElement.Q("createContainer") as VisualElement;
        mainContainer = document.rootVisualElement.Q("MainContainer") as VisualElement;


        createBttn.RegisterCallback<ClickEvent>(onCreateClick);
        joinBttn.RegisterCallback<ClickEvent>(JoinGame);
        listBttn.RegisterCallback<ClickEvent>(ListGame);
    }

    private void OnDisable()
    {
        createBttn.UnregisterCallback<ClickEvent>(CreateGame);
        joinBttn.UnregisterCallback<ClickEvent>(JoinGame);
        listBttn.UnregisterCallback<ClickEvent>(ListGame);
    }

    void onCreateClick(ClickEvent evt)
    {
        // publicBttn.style.display = DisplayStyle.Flex;
        mainContainer.style.display = DisplayStyle.None;
        createContainer.style.display = DisplayStyle.Flex;

    }
    async void CreateGame(ClickEvent evt)
    {
        await MultiplayerNetwork.Instance.CreateLobby();
        //Loader.LoadNetwork(gameScene);
    }

    async void JoinGame(ClickEvent evt)
    {
        await MultiplayerNetwork.Instance.QuickJoinLobby();
    }

    async void ListGame(ClickEvent evt)
    {
        await MultiplayerNetwork.Instance.ListLobbies();
    }
}


//public static class Loader
//{
//    public static void LoadNetwork(SceneReference gameScene)
//    {
//        NetworkManager.Singleton.SceneManager.LoadScene(gameScene.Name, LoadSceneMode.Single);
//    }
//}