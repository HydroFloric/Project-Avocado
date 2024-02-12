using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;
using Unity.Properties;

/* Don't reference map nodes by their transform position in world space, always use reference.Vec3Location(). transform may not exist in future implementations.
 * 
 * 
 */
public struct mapItem
{
    public mapItem(HexNode n, int dist) //this exists so i dont need to store two seperate dictionary holding distance and connected nodes
    {
        node = n;
        distance = dist;
    }
    public HexNode node;
    public int distance;
}
public class MapManager : MonoBehaviour
{
    private int[,] OddOffset = { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    private int[,] EvenOffset = { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };

    public Dictionary<HexNode, Dictionary<HexNode, mapItem>> PathfindingGraphs = new Dictionary<HexNode, Dictionary<HexNode, mapItem>>(); 


    HexNode[,] map;
    int map_x;
    int map_z;
   public void initMap(GameObject[,] m) { //instead of a game object the input will be whatever map data is created when we eveutally get to that point
        map_x = m.GetLength(0);
        map_z = m.GetLength(1);
        map = new HexNode[map_x, map_z];
        //probably a way more efficient way to convert my GameOjbects to HexNode while maintaining index. 
        foreach(var i in m)
        {
            HexNode item = i.GetComponent<HexNode>();
            map[item._gridPositionX, item._gridPositionZ] = item;
        }
        foreach(var item in map)
        {
            item.SetNeightbours(findNeightbours(item));
        }
 
    }
    //this will create references for each connected node, we could add extra parameters for impassable objects but i dont like that! better to look up even if slower.
    //Doesn't matter anymore the new algo implements it automatically. 
    public HexNode[] findNeightbours(HexNode i) 
    {
        HexNode[] neighbours = new HexNode[6];
        var x = i._gridPositionX;
        var z = i._gridPositionZ;
        int[,] points;
        if(x % 2 == 0) { points = EvenOffset; }
        else { points = OddOffset; }
        for (int j = 0; j < points.GetLength(0); j++)
        {

            if (x + points[j, 0] >= map_x || x + points[j, 0] < 0) { neighbours[j] = null; continue; }
            if (z + points[j, 1] >= map_z || z + points[j, 1] < 0) { neighbours[j] = null; continue; }
            neighbours[j] = map[x + points[j, 0], z + points[j, 1]];

        }
        return neighbours;
    }
    //This is only half the implementation, ill need to create another pass that stores weights as this only stores directional information at the moment 
    //Technically this is BFS instead of A* since we lack any heuristics or weights
    //Added Weights now its just dykstras, may not add heuristics in this iteration as this should work well enough for now.

    public bool GenerateFlowField(HexNode goal)
    {
        if (PathfindingGraphs.ContainsKey(goal)) { return true; }

        var tempDict = new Dictionary<HexNode, mapItem>();
        var queue = new PriorityQueue<HexNode, int>();

        queue.Enqueue(goal, 0);
        tempDict.Add(goal, new mapItem(null, 0));


        while (queue.Count != 0) {
            var cur = queue.Dequeue();
            
            foreach(var obj in cur.getNeightbours())
            {
                if(obj == null) continue;
                var cost = obj.terrainDif + tempDict[cur].distance;

                mapItem temp = new mapItem(null, -1);
                tempDict.TryGetValue(obj, out temp); 
                if (!tempDict.ContainsKey(obj) || ( cost < temp.distance && temp.distance != -1)){ //i know this is redundent but "structs cannot be initilized to null in this version of .net"
                    tempDict.Add(obj, new mapItem(cur, cost));
                    queue.Enqueue(obj, cost);
                }
            }
        }
       
        PathfindingGraphs.Add(goal, tempDict);
        return true;
    }
    public HexNode nextNodeInPath(HexNode goal, HexNode cur)
    {
        if(goal == null ) return null;
        if(cur == null ) return null;

        GenerateFlowField(goal);
        mapItem nextNode;
        PathfindingGraphs[goal].TryGetValue(cur, out nextNode);
        return nextNode.node;
    }
    public HexNode findClosetNode(Vector3 v) //I need to find a better way to do this besides looping over the entire map, Shouldn't be called too often!
    {
        float dist = -1;
        HexNode closet = null;
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                var temp = Vector3.Distance(map[i, j].Vec3Location(), v);
                if ((temp < dist || dist == -1) && map[i,j].terrainDif != 100) { dist = temp; closet = map[i, j]; }
            }
        }
        return closet;
    }
    public HexNode getNode(int x, int z)
    {
        return map[x, z];
    }
    public void GenerateAll()
    {
        foreach(var node in map)
        {
            GenerateFlowField(node);
        }
    }
}
