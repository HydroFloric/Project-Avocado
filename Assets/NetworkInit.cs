using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkInit : NetworkBehaviour
{
    // Network variable to track the number of players connected
    public NetworkVariable<int> playercount = new NetworkVariable<int>();

    // Network variable to store the random seed for map generation (used by server)
    public NetworkVariable<int> seed = new NetworkVariable<int>();


    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            seed.Value = 0;
            playercount.Value = 0;
        }
        // Debug message to check if spawning is successful (commented out for clarity)
        // Debug.Log("We are here " + playercount.Value);

        // Find all Player objects as children of the "PlayerManager" GameObject
        var players = GameObject.Find("PlayerManager").GetComponentsInChildren<Player>();

        // If we have a player object corresponding to the playercount value
        if (players.Length > playercount.Value)
        {
            // Set the playerName of that player object to the OwnerClientId (the client's unique ID)
            players[playercount.Value].playerName = OwnerClientId.ToString();
        }

        // Only run the following code if this is the server
        if (IsServer)
        {
            // Generate a random seed value for map generation
            seed.Value = Random.Range(0, 99999);

            // Get the HexagonMapGenerator component from this GameObject
            var mg = GetComponent<HexagonMapGenerator>();

            // Set the noise seed for the HexagonMapGenerator using the network variable
            mg.SetNoiseSeed(seed.Value);
        }

        // Call the GenerateMap function (implementation likely defined elsewhere)
        GenerateMap();

        // Loop through all the Player objects found earlier
        for (int i = 0; i < players.Length; i++)
        {
            // Get the base location for this player from the MapBaseGenerator component (implementation likely defined elsewhere)
            var temp = GetComponent<MapBaseGenerator>().GetBaseLocation(i);

            // Find the corresponding map node using the MapManager component (likely in a Manager GameObject)
            var temp1 = GameObject.Find("Manager").GetComponent<MapManager>().getNode(temp.x, temp.y);

            // Set the base location and camera for the player object
            players[i].SetBaseLocation(temp1);
            players[i].SetCamera();
        }

        // Call the IncPlayerServerRpc function on the server to increment the player count
        IncPlayerServerRpc();
    }

    // Server-side RPC function to increment the player count (can be called from any client)
    [ServerRpc(RequireOwnership = false)]
    public void IncPlayerServerRpc()
    {
        playercount.Value++;
    }


    // Client-side RPC function to set the current player number on the client
    [ClientRpc]
    public void SetPlayerClientRpc(int player)
    {
        // Find the SwitchPlayer component in the "PlayerManager" GameObject and set the currentPlayer value
        GameObject.Find("PlayerManager").GetComponent<SwitchPlayer>().currentPlayer = player;
    }

    // Function to generate the map (implementation likely defined elsewhere)
    public void GenerateMap()
    {
        // Debug message indicating the search for the Client object (commented out for clarity)
        // Debug.Log("Finding Client object");

        // Get the HexagonMapGenerator component and set the noise seed using the network variable
        GetComponent<HexagonMapGenerator>().SetNoiseSeed(seed.Value);

        // Call the GenerateHexagonGrid function on the HexagonMapGenerator component (likely generates the map visuals)
        GetComponent<HexagonMapGenerator>().GenerateHexagonGrid();

        // Get the MapManager component from the "Manager" GameObject and initialize the map using the tileMap from HexagonMapGenerator
        GameObject.Find("Manager").GetComponent<MapManager>().initMap(GetComponent<HexagonMapGenerator>().tileMap);

        // Initialize other map generation components (MapBaseGenerator and MapGenerateCrystals) assuming they exist on this GameObject
        GetComponent<MapBaseGenerator>().init();
        GetComponent<MapGenerateCrystals>().init();
    }
}