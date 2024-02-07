using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* This class will hold a reference to all active entities we probably need an entity class to build everything off of incase we need pathfinding for tower units
 * For now we will use a queue and just dynamically adjust how many ms is spent on dealing with moving units
 * Later we can use some multithreading magic to make it more performant
 *
 *
 */
public class PathFinderManager : MonoBehaviour
{
    public Queue<jobPathfinding> jobs = new Queue<jobPathfinding>();
    float maxTime = 5.0f; //5 ms max 
    MapManager mapManager;

    private void Start()
    {
        mapManager = GetComponent<MapManager>();
    }
    public void ScheduleJob(EntityBase e, HexNode goal)
    {
        if(e.currentLocation == null)
        {
            e.currentLocation = mapManager.findClosetNode(new Vector3(e.x, e.y, e.z)); //jus making sure the object knows where it is in map space I really wish i could think of a way to generalize the closetNode instead of looping
        }
        jobs.Enqueue(new jobPathfinding(e, goal));
    }
    void DequeueJob(jobPathfinding j)
    {
        var currentNode = j.entity.currentLocation;
        var pathingTo = j.entity.pathingTo;
        var goalNode = j.goal;
        if (currentNode == null) { currentNode = mapManager.findClosetNode(transform.position); } //dont call this often it takes almost 2000 ticks

        if (pathingTo == null)
        {
            pathingTo = mapManager.nextNodeInPath(goalNode, currentNode);
            j.entity.pathingTo = pathingTo;
        }
        if (Vector3.Distance(pathingTo.Vec3Location(), j.entity.toVec3()) < 1)
        {
            j.entity.currentLocation = pathingTo;
            j.entity.pathingTo = null;
        }
    }

    private void FixedUpdate() //may add actually multithreading in future but this works for now, each frame will at max only use a certain amount of time for calculations
    {
        float t = Time.time;
        float _frameTime = t + maxTime;

        while(t < _frameTime && jobs.Count > 0)
        {
            DequeueJob(jobs.Dequeue());
            t += (Time.time - t);
        }
    }
}
public struct jobPathfinding
{
    public jobPathfinding(EntityBase e, HexNode n) { entity = e; goal = n; }
    public EntityBase entity;
    public HexNode goal;
}