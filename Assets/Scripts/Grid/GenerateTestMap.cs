using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateTestMap : MonoBehaviour
{
    public GameObject hexagon;

    public GameObject[,] map;
    public MapManager mapManager;
    public int x = 10;
    public int z = 10;
    // Start is called before the first frame update
    void Start()
    {
        map = new GameObject[x, z];
        
        float Real_x = 0;
        float Real_z = 0;
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++) {
                Debug.Log("i: " + i + " j: " + j + "real_x: " + Real_x + " real_z: " + Real_z );
                map[i, j] = Instantiate(hexagon);
                HexNode node = map[i, j].GetComponent<HexNode>();
                node.initialize(Real_x, Real_z, i, j);

                map[i, j].transform.position = new Vector3(Real_x, 0, Real_z);
                
                Real_z += 2f;
            }
            if (i % 2 != 0) { Real_z = 0; }
            else { Real_z = 1; }
            Real_x += 1.5f;
        }

        mapManager = new MapManager(map);
        Debug.Log("hello");
    }

}
