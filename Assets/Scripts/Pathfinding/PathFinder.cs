using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public GameObject goal;
    public GameObject MapManagerGO;

    private MapManager mapManager;
    private TempGoal tp;
    public profileUtility profileUtility;
    public HexNode currentNode;
    public HexNode goalNode;
    public HexNode pathingTo;


    

    private void Start()
    {
        profileUtility = null;
        this.TryGetComponent<profileUtility>(out profileUtility);
        mapManager = MapManagerGO.GetComponent<MapManager>();
        tp = goal.GetComponent<TempGoal>();
    }
    void FixedUpdate()
    {
        if(profileUtility != null) profileUtility.startTimer();
        if (currentNode == null) { currentNode = mapManager.findClosetNode(transform.position); }
        if (goalNode == null || tp.currentNode != goalNode) {goalNode = tp.currentNode;}
        if (pathingTo == null) {
            pathingTo = mapManager.nextNodeInPath(goalNode, currentNode);
        }
        if (Vector3.Distance(pathingTo.transform.position, this.transform.position) < 1)
        {
            currentNode = pathingTo;
            pathingTo = null;
        }
        if (profileUtility != null) profileUtility.stopTimer();
       
    }
}
