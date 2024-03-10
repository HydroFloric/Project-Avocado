using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerInput : MonoBehaviour
{
    PipeLineManager pipeLineManager;
    MapManager mapManager;

    TowerPlayer player;
    bool canPlace = false;
    GameObject selectedTower;

    Vector2 set_mouse = Vector2.negativeInfinity;
    Vector2 cur_mouse;
    GameObject pointer;
    GameObject cur_pointer;
    Rect rect;
    private void Start()
    {
        pipeLineManager = GetComponent<PipeLineManager>();
        player = GetComponent<TowerPlayer>();
        mapManager = GetComponentInParent<MapManager>();
        pointer = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cylinder));

        cur_pointer = pointer;
    }
    // Update is called once per frame
    void Update()
    {
        cur_pointer.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
        var temp = cur_pointer.transform.position;
        temp.y = 0;
        cur_pointer.transform.position = temp;

        
        if (pointer != cur_pointer)
        {
            getPointerColor();
            if(Input.GetMouseButtonDown(0)) {
                player.AddTower(selectedTower, mapManager.findClosetNode(cur_pointer.transform.position));
            }
            if(Input.GetMouseButtonDown(1))
            {
                pointer.transform.position = cur_pointer.transform.position;
                Destroy(cur_pointer);
                pointer.SetActive(true);
                cur_pointer = pointer;
            }
        }
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
    public void setPointer(GameObject c)
    {
        selectedTower = c;
        cur_pointer = Instantiate(c);
        pointer.SetActive(false);
        foreach(Renderer r in cur_pointer.GetComponentsInChildren<Renderer>())
        {
            r.material.color = new Color(1, 0, 0, 0.5f);
        }
        
    }
    public void getPointerColor()
    {
        Color c;
        canPlace = pipeLineManager.contains(cur_pointer.transform.position);
        if (canPlace)
        {
            c = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            c = new Color(1, 0, 0, 0.5f);
        }
        foreach (Renderer r in cur_pointer.GetComponentsInChildren<Renderer>())
        {
            r.material.color = c;
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