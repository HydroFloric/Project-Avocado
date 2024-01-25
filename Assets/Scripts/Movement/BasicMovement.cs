using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Vector3 pathingTo;
    PathFinder pathFinder;

    public float speed = 1f;
    void Start()
    {
        pathFinder = GetComponent<PathFinder>();
        if (pathFinder.pathingTo != null)
        {
            pathingTo = pathFinder.pathingTo.Vec3Location();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, (pathingTo - new Vector3(transform.position.x, 0, transform.position.z)).normalized);
        
        if (pathFinder.pathingTo != null)
        {
            pathingTo = pathFinder.pathingTo.Vec3Location();
            transform.Translate((pathingTo - new Vector3(transform.position.x,0,transform.position.z)).normalized * speed * Time.deltaTime);
        }

    }
}
