using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Try not to actually do any work in this class just add attributes that entities have
 * (spd, resistances, damage)
 */

public class EntityBase : MonoBehaviour
{
    public float x, y, z;

    public HexNode currentLocation;
    public HexNode pathingTo;
    
    EntityBase(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public Vector3 toVec3()
    {
        return new Vector3(x, y, z);
    }
    public void SetVec3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

}
