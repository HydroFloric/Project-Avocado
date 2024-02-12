using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//this whole thing is getting scrapped. we are gonna use a queue!
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

    public bool debug = false;

    

    private void Start()
    {
        profileUtility = null;
        this.TryGetComponent<profileUtility>(out profileUtility);
        mapManager = MapManagerGO.GetComponent<MapManager>();
        tp = goal.GetComponent<TempGoal>();
    }
    void FixedUpdate()
    {
      
        if (currentNode == null) { currentNode = mapManager.findClosetNode(transform.position); } //dont call this often it takes almost 2000 ticks
        if (goalNode == null || tp.currentNode != goalNode) { goalNode = tp.currentNode; }
        if (pathingTo == null) {
            pathingTo = mapManager.nextNodeInPath(goalNode, currentNode);
        }
        if (Vector3.Distance(pathingTo.Vec3Location(), this.transform.position) < 1)
        {
            currentNode = pathingTo;
            pathingTo = null;
        }
        if (profileUtility != null) {profileUtility.stopTimer();
            HexNode temp = pathingTo;
            HexNode cur = currentNode;
            while(temp != null) { 
                temp = mapManager.nextNodeInPath(goalNode, temp);
                profileUtility.path(cur, temp);
                cur = temp;
            }
            
        }
       
    }
}
