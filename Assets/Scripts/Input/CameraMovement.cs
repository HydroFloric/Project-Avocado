using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //SwarmUI ui;

    float map_x;
    float map_y;

    private void Start()
    {
        //ui = GetComponentInParent<SwarmUI>();
       
    }
    void Update()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += new Vector3(0, -1, 0);
        } 
        if (Input.GetKey(KeyCode.A))
        {
            movement += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += new Vector3(1, 0, 0);
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            GetComponent<Camera>().orthographicSize -= 0.75f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            GetComponent<Camera>().orthographicSize += 0.75f;
        }
        transform.Translate(movement);
        //ui.UpdateCameraPosition(new Vector2(movement.x, movement.y));
        
    }
}
