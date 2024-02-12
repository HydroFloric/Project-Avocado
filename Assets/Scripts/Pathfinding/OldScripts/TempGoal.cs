using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoal : MonoBehaviour
{
    public GameObject mapManager;
    public HexNode currentNode;

    private Vector3 prevPos = Vector3.zero;

    private void Start()
    {
        prevPos = transform.position;
        currentNode = mapManager.GetComponent<MapManager>().findClosetNode(prevPos);
    }
    void LateUpdate()
    {
        if (prevPos != transform.position)
        {
            prevPos = transform.position;
            currentNode = mapManager.GetComponent<MapManager>().findClosetNode(prevPos);
        }
    }
}
