using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class networkUI : MonoBehaviour
{


    [SerializeField] private Button clientBttn;
    [SerializeField] private Button hostBttn;

    private void Awake()
    {

        hostBttn.onClick.AddListener(() =>
           {
               NetworkManager.Singleton.StartHost();

               Debug.Log("Host started.");
           });

        //SubscribeToNetworkEvents();

        clientBttn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started.");
        });

        SubscribeToNetworkEvents();
       
    }

    public void SubscribeToNetworkEvents()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            Debug.Log("OnServerStarted: Server has started.");
        };

       

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log($" Successfully connected to the server. Local Client ID: {clientId}");
            }
            else
            {
                Debug.Log($"A client connected with Client ID: {clientId}");
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            Debug.Log($" A client disconnected with Client ID: {clientId}");
        };
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= () => { };
            NetworkManager.Singleton.OnClientConnectedCallback -= (clientId) => { };
            NetworkManager.Singleton.OnClientDisconnectCallback -= (clientId) => { };
        }
    }

}
