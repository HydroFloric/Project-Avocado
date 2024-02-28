using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Vector3 currentDir;
    EntityBase entityBase;
    //true oop would have me extending the entity base class with this but that sounds like alot of work for no performance!
    private float speed = 1f;

    public float timeSincelastPositionUpdate = 0f;
   
    void Start()
    {
        entityBase = GetComponent<EntityBase>();
        speed = entityBase.speed;
    }

    // Update is called once per frame
    void Update()
    {

        entityBase.SetVec3(entityBase.toVec3() + currentDir.normalized * speed * Time.deltaTime);
        transform.position = entityBase.toVec3();


        //lerp this eventually!
        var temp = currentDir.normalized;
        float angle = Mathf.Atan2(temp.x, temp.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(-90,angle, 0);
        
        
    }
    void FixedUpdate()
    {
        if(entityBase.state == State.idle)
        {
            currentDir = Vector3.zero;
        }
        if (entityBase.state == State.moving)
        {
            Debug.DrawRay(transform.position, currentDir * 2, Color.magenta);
            FindPath();

        }
        ObjectAvoidance();
        
    }
    void FindPath()
    {
        if(entityBase.pathingTo == null)
        {
            currentDir = Vector3.zero;
        }
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
    void ObjectAvoidance()
    {
        float close_dx = 0;
        float close_dz = 0;
       Collider[] hits = Physics.OverlapSphere(entityBase.toVec3(), speed * 0.2f, LayerMask.GetMask("Swarm"));
       
       for(int i = 0; i < hits.Length; i++)
        {
            EntityBase temp;
            hits[i].gameObject.TryGetComponent<EntityBase>(out temp);  
            if(temp == null) continue;

            close_dx += entityBase.x - temp.x;
            close_dz += entityBase.z - temp.z;
            
       }
        currentDir.x += close_dx * 1.0f;
        currentDir.z += close_dz * 1.0f;

        entityBase.x += close_dx * 1.0f;
        entityBase.z += close_dz * 1.0f;
    }
}
