using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public GameObject goal;
    public GameObject MapManagerGO;
    private MapManager mapManager;

    public HexNode currentNode;
    public HexNode goalNode;
    public HexNode pathingTo;

    private void Start()
    {
        mapManager = MapManagerGO.GetComponent<MapManager>();

    }
    void FixedUpdate()
    {
        if (currentNode == null) { currentNode = mapManager.findClosetNode(transform.position); }
        if (goalNode == null || goal.GetComponent<TempGoal>().currentNode != goalNode) {goalNode = goal.GetComponent<TempGoal>().currentNode;}
        if (pathingTo == null) { pathingTo = mapManager.nextNodeInPath(goalNode, currentNode); }

        if(currentNode != mapManager.findClosetNode(transform.position))
        {
            currentNode = mapManager.findClosetNode(transform.position);
            pathingTo = null;
        }
    }
    
}
