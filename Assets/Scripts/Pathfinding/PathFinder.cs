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
        if (goalNode == null || goal.GetComponent<TempGoal>().hasChanged)
        {
            goalNode = goal.GetComponent<TempGoal>().currentNode;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<HexNode>() != null)
        {
            currentNode = collision.gameObject.GetComponent<HexNode>();
            pathingTo = mapManager.nextNodeInPath(goalNode, currentNode);
        }
        
    }
}
