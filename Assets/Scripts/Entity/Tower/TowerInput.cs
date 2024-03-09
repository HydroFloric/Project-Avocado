using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInput : MonoBehaviour
{
    PipeLineManager pipeLineManager;
    TowerPlayer player;
    Vector2 set_mouse = Vector2.negativeInfinity;
    Vector2 cur_mouse;
    GameObject pointer;
    Rect rect;
    private void Start()
    {
        pipeLineManager = GetComponent<PipeLineManager>();
        pointer = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cylinder));
    }
    // Update is called once per frame
    void Update()
    {
        pointer.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, Camera.main.transform.position.z));
        var temp = pointer.transform.position;
        temp.y = 0;
        pointer.transform.position = temp;
        if (Input.GetMouseButtonDown(0))
        {
            set_mouse = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && set_mouse != Vector2.negativeInfinity)
        {
            cur_mouse = Input.mousePosition;
            DrawSelectionBox();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(Input.mousePosition);
            pipeLineManager.setGoal(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            set_mouse = Vector2.negativeInfinity; cur_mouse = Vector2.zero;
            rect.Set(-1, -1, -1, -1);
        }
    }
    private void OnGUI()
    {
        GUI.Box(rect, "");
    }
    void DrawSelectionBox()
    {
        float BoxWidth = cur_mouse.x - set_mouse.x;
        float BoxHeight = set_mouse.y - cur_mouse.y;
        float BoxX = set_mouse.x;
        float BoxY = (Screen.height - cur_mouse.y) - BoxHeight;
        rect.Set(BoxX, BoxY, BoxWidth, BoxHeight);

    }
}
