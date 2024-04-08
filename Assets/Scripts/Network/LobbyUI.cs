//using Eflatun.SceneReference;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine.SceneManagement;
//using UnityEngine;
//using UnityEngine.UI;

//public class LobbyUI : MonoBehaviour
//{
//    Button createLobbyBttn;
//    Button joinLobbyBttn;
//    [SerializeField] SceneReference gameScene;

//    private void Awake()
//    {
//        createLobbyBttn.onClick.AddListener(CreateGame);
//        joinLobbyBttn.onClick.AddListener(JoinGame);
//    }

//    async void CreateGame()
//    {
//        await MultiplayerNetwork.Instance.CreateLobby();
//        Loader.LoadNetwork(gameScene);
//    }

//    async void JoinGame()
//    {
//        await MultiplayerNetwork.Instance.QuickJoinLobby();
//    }
//}

