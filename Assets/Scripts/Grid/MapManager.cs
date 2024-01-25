using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapManager : MonoBehaviour
{
    private int[,] OddOffset = { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    private int[,] EvenOffset = { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };
    public Dictionary<HexNode, Dictionary<HexNode, HexNode>> DMaps = new Dictionary<HexNode, Dictionary<HexNode, HexNode>>();
    HexNode[,] map;
    int map_x;
    int map_z;
   public void initMap(GameObject[,] m) {
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
        GenerateAll();
   }
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
    //Technically this is BFS instead of A* since we lack any heuristics 
    public bool GenerateFlowField(HexNode goal)
    {
        if (DMaps.ContainsKey(goal)) { return true; }

        var tempDict = new Dictionary<HexNode, HexNode>(); 
        var queue = new Queue<HexNode>();

        queue.Enqueue(goal);
        tempDict.Add(goal, null);

        while (queue.Count != 0) {
            var cur = queue.Dequeue();
            foreach(var obj in cur.getNeightbours())
            {
                if(obj == null) continue;
                if (tempDict.ContainsKey(obj)) continue;
                queue.Enqueue(obj);
                tempDict.Add(obj, cur);
            }
        }
        DMaps.Add(goal, tempDict);
        return true;
    }
    public HexNode nextNodeInPath(HexNode goal, HexNode cur)
    {
        if (!DMaps.ContainsKey(goal)){
            GenerateFlowField(goal);
        }
         var dict = DMaps[goal];
         HexNode nextNode = null;
         dict.TryGetValue(cur, out nextNode);
         return nextNode;
     

    }
    public HexNode findClosetNode(Vector3 v)
    {
        float dist = -1;
        HexNode closet = null;
        foreach(var node in map)
        {
            var temp = Vector3.Distance(node.transform.position,v);
            if(temp < dist || dist == -1) {dist = temp; closet = node;}
            Debug.Log(dist);
        }
        return closet;
    }
    public void GenerateAll()
    {
        foreach(var node in map)
        {
            GenerateFlowField(node);
        }
    }
}
