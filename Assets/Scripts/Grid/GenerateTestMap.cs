
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateTestMap : MonoBehaviour
{
    public GameObject hexagon;
    
    public GameObject[,] map;
    MapManager mapManager;
    public int x = 10;
    public int z = 10;
    // Start is called before the first frame update
    void Start()
    {
        map = new GameObject[x, z];
        mapManager = this.GetComponent<MapManager>();
        float Real_x = 0;
        float Real_z = 0;
        for(int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++) {

                map[i, j] = Instantiate(hexagon);
                map[i, j].name = "x: " + i + " z: " + j;
                HexNode node = map[i, j].GetComponent<HexNode>();
                node.initialize(Real_x, Real_z, i, j);

                map[i, j].transform.position = new Vector3(Real_x, 0, Real_z);
                
                Real_z += 2f;
            }
            if (i % 2 != 0) { Real_z = 0; }
            else { Real_z = 1; }
            Real_x += 1.5f;
        }
        mapManager.initMap(map);
    }

}
