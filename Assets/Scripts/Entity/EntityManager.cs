using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    // Reference to the JobManager component used for pathfinding
    public JobManager pathFinder;

    // Reference to the MapManager component used for map data
    public MapManager mapManager;



    // Dictionary to store goals (target locations) for each entity
    private Dictionary<EntityBase, HexNode> goals = new Dictionary<EntityBase, HexNode>();

    // List to store all managed entities (game objects representing units)
    public List<EntityBase> entities = new List<EntityBase>();

    // List to store currently selected units
    public List<EntityBase> selectedUnits = new List<EntityBase>();

    // Array of prefabs for different unit types (consider moving this to a data table or resource manager)
    public GameObject[] types = null;

    // Start method called when the game object is instantiated
    private void Start()
    {
        // Get references to components
        pathFinder = GetComponent<JobManager>();
        mapManager = GetComponent<MapManager>();
        // Spawn entities if prefab types are provided
        if (types != null)
        {
            for (int i = 0; i < types.Length; i++)
            {
                //Vector3 tempPos = mapManager.getNode(GameObject.Find("NetworkIntilizer").GetComponent<MapBaseGenerator>().GetBaseLocation(1);
                //spawn(tempPos, i);
            }

            // Create a copy of the entities list for selected units (temporary solution, might be improved later)
            selectedUnits = entities.CloneViaSerialization();
        }
    }

    // FixedUpdate method called at a fixed interval (often used for physics)
    private void FixedUpdate()
    {
        move();
    }

    // Function to select units within a specific screen area
    public void select(Vector2 a, Vector2 c)
    {
        // Calculate corner points for the selection area
        Vector2 b = new Vector2(c.x, a.y);
        Vector2 d = new Vector2(a.x, c.y);

        // Clear the previously selected units
        selectedUnits.Clear();

        // Loop through all entities
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];

            // Convert entity's world position to screen space coordinates
            Vector2 temp = new Vector2(entity.x, entity.y);
            temp = Camera.main.WorldToScreenPoint(temp);
            temp = new Vector2(Screen.height - temp.y, temp.x);

            // Perform calculations to check if the entity is within the selection area (based on vector dot products)
            var am_ab = Vector2.Dot((a * temp), (a * b));
            var ab_ab = Vector2.Dot((a * b), (a * b));
            var am_ad = Vector2.Dot((a * temp), (a * d));
            var ad_ad = Vector2.Dot((a * d), (a * d));

            // If the entity is within the area, add it to the selected units list and update UI
            if ((0 < am_ab && am_ab < ab_ab) && (0 < am_ad && am_ad < ad_ad))
            {
                selectedUnits.Add(entity);
                Debug.Log("Entity selected!");
            }
        }

        GameObject.Find("SwarmManager").GetComponentInChildren<SwarmUI>().UpdateUI(selectedUnits); // Update UI with the newly selected units
    }

    // Function to set a goal (target location) for all selected units
    public void setGoal(Vector3 origin)
    {
        // Find the closest node to the origin point using the MapManager
        HexNode node = mapManager.findClosetNode(origin);

        // Remove any null entities from the selected units list
        selectedUnits.RemoveAll(a => a == null);

        // Loop through all selected units
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            EntityBase entity = selectedUnits[i];

            Debug.Log("Obj: " + entity.name + " goal node set: " + node.name);

            // Set the goal node for the entity in the goals dictionary
            goals[entity] = node;
            // Update the entity's state to moving and log the state change
            entity.state = State.moving;
            Debug.Log("Obj State:" + entity.name + ": " + entity.state);
        }
    }
    // Function to move entities (likely triggers pathfinding jobs)
    public void move()
        {
            // Loop through all entities
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];

                // Check if the entity has a goal and hasn't reached it yet
                if (!goals.ContainsKey(entity) || entity.currentLocation == goals[entity])
                {
                    entity.state = State.idle; // Set state to idle if no goal or reached goal
                }

                // If the entity is not null and has a moving state
                if (entity != null && entity.state == State.moving)
                {
                    // If the entity's current location is unknown, find the closest node
                    if (entity.currentLocation == null)
                    {
                        entity.currentLocation = mapManager.findClosetNode(new Vector3(entity.x, entity.y, entity.z));
                        Debug.Log("Ensuring entity knows its location: " + entity.name);
                    }

                    // Schedule a pathfinding job for this entity using the JobManager
                    pathFinder.ScheduleJob(new jobPathfinding(entity, goals[entity], mapManager));

                    // Draw a visual line for debugging purposes (shows path from current to goal)
                    Debug.DrawLine(entity.currentLocation.Vec3Location(), goals[entity].Vec3Location(), Color.green);
                }
            }
        }

        // Function to spawn an entity (game object) of a specific type
    public void spawn(Vector3 WorldCords, int type)
    {
            if (GameObject.Find("SwarmManager").GetComponent<SwarmPlayer>().EntityLimit > entities.Count)
            {
                // Instantiate the prefab based on the type index
                var temp = Instantiate(types[type]);

                // Set the position of the spawned entity
                temp.transform.position = WorldCords;

                // Get the EntityBase component from the instantiated game object
                var bas = temp.GetComponent<EntityBase>();

                // Call a function on the EntityBase component to set its internal position (might be specific to your implementation)
                bas.SetVec3(WorldCords);

                // Add the new entity to the entities list for management
                entities.Add(bas);
            }
    }
}