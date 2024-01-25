using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoal : MonoBehaviour
{
    public bool hasChanged = false;
    public HexNode currentNode;

    void FixedUpdate()
    {

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit data;
        Physics.Raycast(ray, out data);
        GameObject hit = data.transform.gameObject;
        if (hit != null && hit.GetComponent<HexNode>() != null)
        {
            var tempNode = hit.GetComponent<HexNode>();
            if (currentNode != tempNode)
            {
                currentNode = tempNode;
                hasChanged = true;
            }
            if(currentNode == tempNode) { hasChanged = false; }

        }


    }
}
