using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmInput : MonoBehaviour
{
    EntityManager entityManager;

    Vector2 set_mouse = Vector2.negativeInfinity;
    Vector2 cur_mouse;
    Rect rect;
    private void Start()
    {
        entityManager = GetComponent<EntityManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            set_mouse = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            entityManager.select(set_mouse, cur_mouse);
            set_mouse = Vector2.negativeInfinity; cur_mouse = Vector2.zero;
            rect.Set(0,0,0,0);
        }
        if (Input.GetMouseButton(0) && set_mouse != Vector2.negativeInfinity)
        {
            cur_mouse = Input.mousePosition;
            DrawSelectionBox();
        }
        if (Input.GetMouseButtonDown(1)) {
            entityManager.setGoal(Input.mousePosition);
        }
    }
    private void OnGUI()
    {
        GUI.Box(rect, "");
    }
    void DrawSelectionBox()
    {
        float BoxWidth = cur_mouse.x - set_mouse.x;
        float BoxHeight = set_mouse.y - cur_mouse.y ;
        float BoxX = set_mouse.x;
        float BoxY = (Screen.height - cur_mouse.y) - BoxHeight;
        rect.Set(BoxX,BoxY,BoxWidth,BoxHeight);
       
    }
}
