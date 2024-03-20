using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkInit : NetworkBehaviour
{
    NetworkVariable<int> playercount = new NetworkVariable<int>(0);
    NetworkVariable<int> seed = new NetworkVariable<int>(0);
    NetworkList<Vector2> v = new NetworkList<Vector2>();
    

    public override void OnNetworkSpawn()
    {
    
        
        Debug.Log("We are here " + playercount.Value);
        var players = GameObject.Find("PlayerManager").GetComponentsInChildren<Player>();
        players[playercount.Value].playerName = OwnerClientId.ToString();
       
        if (IsServer)
        {
            seed.Value = Random.Range(0, 99999);
            
            var mg = GetComponent<HexagonMapGenerator>();
            int _crystalNum = (int)mg._numCrystals; //why is num crystals a float?
            var _x = mg._MapWidth;
            var _y = mg._MapHeight;
            for (int i = 0; i < _crystalNum; i++)
            {

              v.Add(new Vector2(Random.Range(0, _x), Random.Range(0, _y)));
              
            }
        }
        GenerateMap();

        for (int i = 0; i < players.Length; i++)
        {
            var temp = GetComponent<MapBaseGenerator>().GetBaseLocation(i);
            var temp1 = GameObject.Find("Manager").GetComponent<MapManager>().getNode(temp.x, temp.y);

            players[i].SetBaseLocation(temp1);
            players[i].SetCamera();
        }
        IncPlayerServerRpc();
    }
    [ServerRpc(RequireOwnership = false)] public void IncPlayerServerRpc()
    {
        playercount.Value++;
    }
    [ServerRpc(RequireOwnership = false)] public void SetPlayerServerRpc(int player)
    {
        SetPlayerClientRpc(player);
    }
    [ClientRpc] public void SetPlayerClientRpc(int player){
        GameObject.Find("PlayerManager").GetComponent<SwitchPlayer>().currentPlayer = player;
    }
    public void GenerateMap()
    {
        Debug.Log("Finding Client object");
        GetComponent<HexagonMapGenerator>().SetNoiseSeed(seed.Value);
        GetComponent<HexagonMapGenerator>().GenerateHexagonGrid();

        GameObject.Find("Manager").GetComponent<MapManager>().initMap(GetComponent<HexagonMapGenerator>().tileMap);

        GetComponent<MapBaseGenerator>().init();
        GetComponent<MapGenerateCrystals>().init();
    }
}
