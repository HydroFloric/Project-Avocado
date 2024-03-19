using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    JobManager pathFinder;
    MapManager mapManager;
    //I really shouldn't reference swarmUI here and shunt it into a player class! player classes dont exist so we do it here for now
    SwarmUI ui;
    Dictionary<EntityBase, HexNode> goals = new Dictionary<EntityBase, HexNode>();

    public List<EntityBase> entities = new List<EntityBase>();
    List<EntityBase> selectedUnits = new List<EntityBase>();


    public GameObject[] types = null; //can add different prefabs/models 
    private void Start()
    {
        pathFinder = GetComponent<JobManager>();
        mapManager= GetComponent<MapManager>();
        ui = GameObject.Find("PlayerManager").GetComponentInChildren<SwarmUI>();
        if (types != null)
        {
            for(int i = 0; i < types.Length; i++) {
                Vector3 tempPos = new Vector3(0, 0.2f, 0);
                spawn(tempPos, i);
            }
            selectedUnits = entities.CloneViaSerialization(); //this looks dumb for right now but in the future it will be nice to already have the code ready for selecting individual unit groups
        }
    }
    private void FixedUpdate()
    {
        move();
    }
    //Function Select
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
        ui.UpdateUI(selectedUnits);
    }
    public void setGoal(Vector2 a)
    {
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(a.x, a.y, Camera.main.transform.position.z));

        HexNode node = mapManager.findClosetNode(origin);
        selectedUnits.RemoveAll(a=> a == null);
        for (int i = 0; i < selectedUnits.Count; i++) {
            Debug.Log("Obj: " + selectedUnits[i].name + "goal node set: " + node.name);
            goals[selectedUnits[i]] = node;
            selectedUnits[i].state = State.moving;
            Debug.Log("Obj State"+ selectedUnits[i].name +": " + selectedUnits[i].state);
        }
    }

    //Function Move queues pathfindingjob for each entity
    public void move()
    {
        for(int i = 0; i < entities.Count; i++)
        {  
            var entity = entities[i];
            if (!goals.ContainsKey(entity) || entity.currentLocation == goals[entity])
            {
                entity.state = State.idle;
            }
            if (entity != null && entity.state == State.moving)
            {
                if (entity.currentLocation == null)
                {
                    entity.currentLocation = mapManager.findClosetNode(new Vector3(entity.x, entity.y, entity.z)); //jus making sure the object knows where it is in map space I really wish i could think of a way to generalize the closetNode instead of looping
                }

                pathFinder.ScheduleJob(new jobPathfinding(entity, goals[entity], mapManager));
                Debug.DrawLine(entity.currentLocation.Vec3Location(), goals[entity].Vec3Location(), Color.green);
            }
        }
        
    }
    public void spawn(Vector3 WorldCords, int type)
    {
        var temp = Instantiate(types[type]);
        temp.transform.position = WorldCords;
        var bas = temp.GetComponent<EntityBase>();
        bas.SetVec3(WorldCords);
        entities.Add(bas);
    }

}
