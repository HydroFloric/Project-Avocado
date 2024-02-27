using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
/* This class will hold a reference to all active entities we probably need an entity class to build everything off of incase we need pathfinding for tower units
 * For now we will use a queue and just dynamically adjust how many ms is spent on dealing with moving units
 * Later we can use some multithreading magic to make it more performant
 *
 *
 */
public class JobManager : MonoBehaviour
{
    public Queue<job> jobs = new Queue<job>();
    float maxTime = 5.0f; //5 ms max 


    public void ScheduleJob(job j)
    {

        jobs.Enqueue(j);
    }
    public 
    void DequeueJob(job j)
    {
        j.exec();
    }

    private void Update() //may add actual multithreading in future but this works for now, each frame will at max only use a certain amount of time for calculations
    {
        float t = Time.time;
        float _frameTime = t + maxTime;

        while(t < _frameTime && jobs.Count > 0)
        {
            DequeueJob(jobs.Dequeue());
            t = Time.time;
        }
    }
}
public struct jobPathfinding : job
{
    public jobPathfinding(EntityBase e, HexNode n, MapManager m) { entity = e; goal = n; mapManager = m ; }
    public EntityBase entity;
    public HexNode goal;
    public MapManager mapManager;
    public void exec()
    {
        var currentNode = entity.currentLocation;
        var pathingTo = entity.pathingTo;
        var goalNode = goal;
        if (currentNode == null) { currentNode = mapManager.findClosetNode(entity.toVec3()); } //dont call this often it takes almost 2000 ticks

        if (pathingTo == null)
        {
            pathingTo = mapManager.nextNodeInPath(goalNode, currentNode);
            entity.pathingTo = pathingTo;
        }
        if (Vector3.Distance(pathingTo.Vec3Location(), entity.toVec3()) < 1)
        {
            entity.currentLocation = pathingTo;
            entity.pathingTo = null;
        }
    }
}

public interface job
{
    public void exec();
}