using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexNode : MonoBehaviour
{
    public HexNode[] neightbours = new HexNode[6]; //0 up, 1 up right, 2 down right, 3 down, 4 down left, 5 up left e.g clockwise from top
   
    public float _positionX = 0; //location in gridArray
    public float _positionZ = 0;
    public int _gridPositionX = 0;
    public int _gridPositionZ = 0;

    public int terrainDif = 0; //higher number increases edge cost of node
    
    public void initialize(float x, float z, int grid_x, int grid_z)
    {
        _positionX= x;
        _positionZ= z;
        _gridPositionX = grid_x;
        _gridPositionZ = grid_z;   

    }
    public Vector3 Vec3Location()
    {
        return new Vector3(_positionX, 0, _gridPositionZ);
    }
    public bool RemoveNeightbour(int index)
    {
        neightbours[index] = null;
        return true;
    }
    public bool AddNeightbour(HexNode i, int index)
    {
        neightbours[index] = i;
        return true;
    }
    public void SetNeightbours(HexNode[] n)
    {
        neightbours = n;
    }
    public HexNode[] getNeightbours()
    {
        return neightbours;
    }

}
