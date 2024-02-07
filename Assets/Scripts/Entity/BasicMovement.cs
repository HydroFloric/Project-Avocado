using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Vector3 currentDir;
    public HexNode goal;
    EntityBase entityBase;
    public float speed = 1f;

    public float timeSincelastPositionUpdate = 0f;
   
    void Start()
    {
        goal = GameObject.FindWithTag("goal").GetComponent<HexNode>();
        entityBase = GetComponent<EntityBase>();
    }

    // Update is called once per frame
    void Update()
    {
        //I have an issue!
        //if i implement this as is it will work but entities will not be able to be manipulated when not in frame
        //if i do it the other way i got write like two functions to get a lerp working
        //i have comprimised with something that looks ugly and barely works right now!
        entityBase.SetVec3(entityBase.toVec3() + currentDir.normalized * speed * Time.deltaTime);
        transform.position = entityBase.toVec3();
    }
    void FixedUpdate()
    {
       

        Debug.DrawRay(transform.position, currentDir * 2, Color.magenta);
        FindPath();
        


    }
    void FindPath()
    {
        if (entityBase.pathingTo != null)
        {
            Vector3 temp;
            temp = entityBase.pathingTo.Vec3Location();
            temp.y = 0;
            Vector3 tempTransform = entityBase.toVec3();
            tempTransform.y = 0;
            currentDir = (temp - tempTransform).normalized;
        }
    }
}
