using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager
{
    private int[,] OddOffset = { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    private int[,] EvenOffset = { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };
    HexNode[,] map;
    int map_x;
    int map_z;
   public MapManager(GameObject[,] m) {
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
}
