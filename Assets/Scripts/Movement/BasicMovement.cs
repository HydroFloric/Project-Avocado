using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Vector3 pathingTo;
    public Vector3 currentDir;
    PathFinder pathFinder;

    public float speed = 1f;
   
    void Start()
    {
        pathFinder = GetComponent<PathFinder>();
        FindPath();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(currentDir.normalized * speed * Time.deltaTime);
    }
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, currentDir * 2, Color.magenta);
        FindPath();
        
    }
    void FindPath()
    {
        if (pathFinder.pathingTo != null)
        {
            pathingTo = pathFinder.pathingTo.Vec3Location();
            pathingTo.y = 0;
            Vector3 tempTransform = this.transform.position;
            tempTransform.y = 0;
            currentDir = (pathingTo - tempTransform).normalized;
        }
    }
}
