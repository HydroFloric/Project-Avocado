using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    SwarmUI ui;

    private void Start()
    {
        ui = GetComponentInParent<SwarmUI>();
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
        if (Input.mouseScrollDelta.y > 0)
        {
            movement += new Vector3(0, 0, 1);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            movement += new Vector3(0, 0, -1);
        }
        transform.Translate(movement);
        ui.UpdateCameraPosition(new Vector2(movement.x, movement.y));
        
    }
}
