
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateTestMap : MonoBehaviour
{
    public GameObject hexagon; //the map system will need to alot more clever than this. A game object takes up too much memory to be used as a map tile efficiently.
                               //the map data will need to be stored as an reference array then we will need to somehow construct polygons where all the meshes are.

    System.Random random = new System.Random();
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
               

                if (random.Next(0, 11) > 8 && (j != 0 && i != 0))
                {
                    node.initialize(Real_x, Real_z, i, j, 100);
                    node.transform.localScale = new Vector3(node.transform.localScale.x, 10, node.transform.localScale.z);
                }
                else
                {
                    //node.initialize(Real_x, Real_z, i, j);
                }
                map[i, j].transform.position = new Vector3(Real_x, 0, Real_z);
                
                Real_z += 2f;
            }
            if (i % 2 != 0) { Real_z = 0; }
            else { Real_z = 1; }
            Real_x += 1.5f;
        }
        mapManager.initMap(map);

        GetComponent<SwarmUI>().ShowMap();

    }

}
