using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    PathFinderManager pathFinder;
    MapManager mapManager;
    Dictionary<EntityBase, HexNode> goals = new Dictionary<EntityBase, HexNode>();

    List<EntityBase> entities = new List<EntityBase>();
    List<EntityBase> selectedUnits = new List<EntityBase>();
    public string tag = "swarm"; //if tower player needs pathing can be reused in future
    public GameObject[] types = null; //can add different prefabs/models 
    private void Start()
    {
        pathFinder = GetComponent<PathFinderManager>();
        mapManager= GetComponent<MapManager>();
        if (types != null)
        {
            var temp = Instantiate(types[0]);
            temp.transform.position = new Vector3(0, 1, 0);
            entities.Add(temp.GetComponent<EntityBase>());
            selectedUnits = entities.CloneViaSerialization(); //this looks dumb for right now but in the future it will be nice to already have the code ready for selecting individual unit groups
            foreach(var entity in entities)
            {
                goals[entity] = entity.currentLocation;
            }
        }
    }
    private void FixedUpdate()
    {
        move(selectedUnits.ToArray());
    }
    //Function Select, wont be implemented for now but will need a function that allows user to select portion of swarm to issue commands
    //Given two 2d cords determins if an object is within that boundary. 
    //forums said this was faster than relying on the physics system...
    public void select(Vector2 a, Vector2 c)
    {
        Vector2 b = new Vector2(c.x, a.y);
        Vector2 d = new Vector2(a.x, c.y);
        selectedUnits.Clear();
        
        for(int i = 0; i < entities.Count; i++)
        {
            var temp = new Vector2(entities[i].x, entities[i].y);
            temp = Camera.main.WorldToScreenPoint(temp);
            temp = new Vector2( (Screen.height - temp.y), temp.x);

            var am_ab = Vector2.Dot((a * temp), (a * b));
            var ab_ab = Vector2.Dot((a * b), (a * b));
            var am_ad = Vector2.Dot((a * temp), (a * d));
            var ad_ad = Vector2.Dot((a * d), (a * d));
            if ((0 < am_ab && am_ab < ab_ab) && (0 < am_ad && am_ad < ad_ad)) //this is gross, i hate that it works!
            {
                selectedUnits.Add(entities[i]);
                Debug.Log("entity selected!");
            }
        }
    }
    public void setGoal(Vector2 a)
    {
        Vector3 origin = Camera.main.ScreenToWorldPoint(a);
        
        HexNode node = mapManager.findClosetNode(origin);

        for(int i = 0; i < selectedUnits.Count; i++) {
            goals[selectedUnits[i]] = node;
        }
    }
    //Function Move queues pathfindingjob for each entity
    public void move(EntityBase[] selected)
    {
        for(int i = 0; i < entities.Count; i++)
        {  
            var entity = entities[i];
            if (entity != null)
            {
                pathFinder.ScheduleJob(entity, goals[entity]);
            }
        }
        
    }

}
