using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    PathFinderManager pathFinder;

    public TempGoal goal; //eventually goal will be based on mouse location or something but for now

    List<EntityBase> entities = new List<EntityBase>();
    List<EntityBase> selectedUnits = new List<EntityBase>();
    public string tag = "swarm"; //if tower player needs pathing can be reused in future
    public GameObject[] types = null; //can add different prefabs/models 
    private void Start()
    {
        goal = GameObject.FindWithTag("goal").GetComponent<TempGoal>();
        pathFinder = GetComponent<PathFinderManager>();

        if (types != null)
        {
            var temp = Instantiate(types[0]);
           
            entities.Add(temp.GetComponent<EntityBase>());
            selectedUnits = entities; //this looks dumb for right now but in the future it will be nice to already have the code ready for selecting individual unit groups
        }
    }
    private void FixedUpdate()
    {
        move(goal.currentNode, selectedUnits.ToArray());
    }
    //Function Select, wont be implemented for now but will need a function that allows user to select portion of swarm to issue commands

    //Function Draw, creates prefab of entity based on type, if visible to camera

    //Function Move queues pathfindingjob for each entity
    public void move(HexNode goal, EntityBase[] selected)
    {
        for(int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity != null)
            {
                pathFinder.ScheduleJob(entity, goal);
            }
        }
        
    }

}
