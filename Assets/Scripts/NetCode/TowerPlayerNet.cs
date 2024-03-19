using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

class TowerPlayerNet : NetworkBehaviour
{

    [ServerRpc(RequireOwnership = false)] public void AskPlacementServerRpc(string selectedTower, Vector2 hexNodeid, Vector3 pipeLocation)
    {
        placeTowerClientRpc(selectedTower,hexNodeid, pipeLocation);
    }
    [ClientRpc] public void placeTowerClientRpc(string selectedTower, Vector2 hexNodeid, Vector3 pipeLocation)
    {
        MapManager m = GameObject.Find("Manager").GetComponent<MapManager>();
        PipeLineManager p = GameObject.Find("Manager").GetComponent<PipeLineManager>();
        GameObject tower = (GameObject)Resources.Load("Prefabs/Tower/" + selectedTower);

        GameObject.Find("TowerManager").GetComponent<TowerPlayer>().AddTower(tower, m.getNode((int)hexNodeid.x, (int)hexNodeid.y), p.GetPipe(pipeLocation, 1f)) ;
    }
    [ServerRpc(RequireOwnership = false)] public void AskSetGoalServerRpc(Vector3 pos)
    {
        SetGoalClientRpc(pos);
    }
    [ClientRpc] public void SetGoalClientRpc(Vector3 pos) 
    {
        PipeLineManager p = GameObject.Find("Manager").GetComponent<PipeLineManager>();
        p.setGoal(pos);
    }
}