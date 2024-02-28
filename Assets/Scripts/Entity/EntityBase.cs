using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Try not to actually do any work in this class just add attributes that entities have
 * (spd, resistances, damage)
 */
public enum State
{
    idle,
    attacking,
    moving
}
public class EntityBase : MonoBehaviour
{
    //Storing this since transformer my not be in existance...
    public float x, y, z;

    public State state = State.idle;

    public float maxhealth = 100;
    public float health = 100.0f;
    public float speed = 1.0f;
    public int damageResist = DamageSystem.NO_ELEMENT;
    public int damageType = DamageSystem.NO_ELEMENT;


    public HexNode currentLocation;
    public HexNode pathingTo;
    
    public EntityBase() 
    {
        x = 0;
        y = 0;
        z = 0;
    }
    public EntityBase(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public EntityBase(float _x, float _y, float _z, int dmgResist, int dmgType, int hp, float spd)
    {
        x = _x;
        y = _y;
        z = _z;

        damageResist = dmgResist;
        damageType = dmgType;
        health = hp;
        speed = spd;
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
