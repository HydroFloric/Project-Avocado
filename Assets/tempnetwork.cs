using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        public string joinCode;




        async void Start()
        {
            await InitializeUnityServices();
        }
        private async Task InitializeUnityServices()
        {
            
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        public void StartButtons()
        {
            //if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            //if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            //if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();

            if (GUILayout.Button("Host"))
            {
                
                CreateAndStartHost();
            }
            if (GUILayout.Button("Client"))
            {


                JoinAndStartClient(joinCode);
            }
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        public async void CreateAndStartHost()
        {
            try
            {
                await CreateRelay();
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create relay or start host: {e}");
            }
        }


        public async Task CreateRelay()
        {
            try
            {
               
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log("Join Code  : " + joinCode);
                //Code.SetText("Join Code : " + joinCode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                //NetworkManager.Singleton.StartHost();

            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }


        public async void JoinAndStartClient(string joinCode)
        {
            try
            {
                await JoinRelay(joinCode);
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join relay or start client: {e}");
            }
        }

        public async Task JoinRelay(string JoinCode)
        {
            try
            {
                Debug.Log("Joining Relay with " + JoinCode);
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                //NetworkManager.Singleton.StartClient();

                Debug.Log("Relay client connected with the code : " + JoinCode);

            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }



    }


   
}