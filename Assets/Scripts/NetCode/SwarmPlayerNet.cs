using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SwarmPlayerNet : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)] public void AskSelectServerRpc(Vector2 mouseWas, Vector2 mouseIs)
    {
        /*
         * add safety checks
         */
        SelectClientRpc(mouseWas, mouseIs);
    }
    [ClientRpc] public void SelectClientRpc(Vector2 mouseWas, Vector2 mouseIs)
    {
        GameObject.Find("Manager").GetComponent<EntityManager>().select(mouseWas, mouseIs);
    }
    [ServerRpc(RequireOwnership = false)] public void AskIssueMoveServerRpc(Vector3 mouseIs)
    {
        IssueMoveClientRpc(mouseIs);
    }
    [ClientRpc] public void IssueMoveClientRpc(Vector3 mouseIs)
    {
        GameObject.Find("Manager").GetComponent<EntityManager>().setGoal(mouseIs);

    }
    [ServerRpc(RequireOwnership = false)] public void UpdateUIServerRpc()
    {

    }
    [ClientRpc] public void UpdateUIClientRpc()
    {

    }

    [ServerRpc(RequireOwnership = false)] public void AskSpawnEntityServerRpc(int i, int x,int z)
    {
        if (i == -1) {

        }
        else
        {
            SpawnEntityClientRpc(i, x, z);
        }
    }
    [ClientRpc] public void SpawnEntityClientRpc(int i, int x, int z)
    {
        if (i == -1)
        {

        }
        else {
            var temp = GameObject.Find("Manager");
            temp.GetComponent<EntityManager>().spawn(temp.GetComponent<MapManager>().getNode(x,z).Vec3Location(), i);
        }
    }



}
