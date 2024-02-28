using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Try not to actually do any work in this class just add attributes that entities have
 * (spd, resistances, damage)
 */

public class EntityBase : MonoBehaviour
{
    //Storing this since transformer my not be in existance...
    public float x, y, z;

    public int type = 0;
    public float health = 100;
    
    public float maxHealth = 100;
    public float speed;
    public enum State
    {
        idle,
        attacking,
        moving
    }
    public State state = State.idle;
    public HexNode currentLocation;
    public HexNode pathingTo;
    

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
