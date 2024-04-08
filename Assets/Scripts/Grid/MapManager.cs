using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;
using Unity.Properties;

/* Don't reference map nodes by their transform position in world space, always use reference.Vec3Location(). transform may not exist in future implementations.
 * 
 * 
 */
public struct mapItem
{
    // This struct stores information about a connected node in the pathfinding graph
    // It combines the node itself and the distance to reach that node
    public mapItem(HexNode n, int dist) { node = n; distance = dist; }
    public HexNode node;  // The connected node
    public int distance;  // Distance from the starting point to this node
}

public class MapManager : MonoBehaviour
{
    // Private variables for debugging purposes (unused in final implementation)
    private Vector3 debug = Vector3.zero;
    private float debug_rad = 0f;

    // Pre-calculated offset arrays for even and odd rows in the hexagonal grid
    // Used for finding neighbouring nodes efficiently
    private int[,] OddOffset = { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };
    private int[,] EvenOffset = { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };

    // Dictionary to store pre-calculated pathfinding graphs for each node
    // Each graph maps connected nodes to `mapItem` structs containing distance information
    public Dictionary<HexNode, Dictionary<HexNode, mapItem>> PathfindingGraphs = new Dictionary<HexNode, Dictionary<HexNode, mapItem>>();

    // Internal representation of the map as a 2D array of HexNode objects
    private HexNode[,] map;
    // Dimensions of the map (width and depth)
    private int map_x;
    private int map_z;

    // Function to initialize the map based on a 2D array of GameObjects (temporary solution)
    public void initMap(GameObject[,] m)
    {
        // Get the dimensions of the input array
        map_x = m.GetLength(0);
        map_z = m.GetLength(1);

        // Create a new 2D array to store HexNode references
        map = new HexNode[map_x, map_z];

        // Convert GameObjects to HexNode references and populate the map array
        foreach (var go in m)
        {
            HexNode item = go.GetComponent<HexNode>();
            map[item._gridPositionX, item._gridPositionZ] = item;
        }

        // Loop through all nodes in the map and call their SetNeighbours function
        // This function likely sets references to neighbouring nodes based on their positions
        foreach (var item in map)
        {
            item.SetNeightbours(findNeightbours(item));
        }
    }

    // Function to find neighbouring nodes for a given HexNode
    // Uses pre-calculated offset arrays based on the node's row parity (even or odd)
    public HexNode[] findNeightbours(HexNode i)
    {
        // Array to store references to neighbouring nodes
        HexNode[] neighbours = new HexNode[6];

        // Get the node's grid position
        var x = i._gridPositionX;
        var z = i._gridPositionZ;

        // Select the appropriate offset array based on row parity
        int[,] points = x % 2 == 0 ? EvenOffset : OddOffset;

        // Loop through the offset points and check for valid neighbours within the map bounds
        for (int j = 0; j < points.GetLength(0); j++)
        {
            int newX = x + points[j, 0];
            int newZ = z + points[j, 1];

            // Check if the new position is within map boundaries
            if (newX >= map_x || newX < 0 || newZ >= map_z || newZ < 0)
            {
                neighbours[j] = null;
                continue;
            }

            // If valid, store the reference to the neighbour node in the array
            neighbours[j] = map[newX, newZ];
        }

        return neighbours;
    }

    // Function to generate a flow field (pre-calculated pathfinding information) for a specific goal node
    // This function implements Dijkstra's algorithm for pathfinding
    public bool GenerateFlowField(HexNode goal)
    {
        // Check if the pathfinding graph for this goal node already exists
        if (PathfindingGraphs.ContainsKey(goal))
        {
            return true;
        }

        // Create a temporary dictionary to store information for
        // Create a temporary dictionary to store information for nodes during pathfinding
        var tempDict = new Dictionary<HexNode, mapItem>();

        // Create a priority queue to efficiently process nodes based on their distances
        var queue = new PriorityQueue<HexNode, int>();

        // Add the goal node to the queue with a distance of 0 (since it's the destination)
        queue.Enqueue(goal, 0);

        // Add the goal node and a corresponding mapItem with distance 0 to the temporary dictionary
        tempDict.Add(goal, new mapItem(null, 0));

        // Loop until the queue is empty (indicating all reachable nodes have been processed)
        while (queue.Count != 0)
        {
            // Dequeue the node with the lowest distance from the queue
            var cur = queue.Dequeue();

            // Loop through all neighbours of the current node
            foreach (var obj in cur.getNeightbours())
            {
                // Skip null neighbours (i.e., nodes outside the map boundaries)
                if (obj == null)
                {
                    continue;
                }

                // Calculate the total cost to reach the neighbour (distance from current + terrain difficulty)
                var cost = obj.terrainDif + tempDict[cur].distance;

                // Create a temporary mapItem to store potential new distance information
                mapItem temp = new mapItem(null, -1);

                // Try to get the existing mapItem for this neighbour from the temporary dictionary
                tempDict.TryGetValue(obj, out temp);

                // Check if the neighbour is not yet in the dictionary or if the new cost is lower
                // (since Dijkstra's algorithm prioritizes shortest paths)
                if (!tempDict.ContainsKey(obj) || (cost < temp.distance && temp.distance != -1))
                {
                    // Update the temporary dictionary with the new distance and predecessor node
                    tempDict.Add(obj, new mapItem(cur, cost));

                    // Enqueue the neighbour into the priority queue with the calculated cost
                    queue.Enqueue(obj, cost);
                }
            }
        }

        // Add the temporary dictionary containing the pathfinding graph for this goal node to the main dictionary
        PathfindingGraphs.Add(goal, tempDict);

        // Successfully generated the flow field
        return true;
    }

    // Function to retrieve the next node in the path towards a goal from a current node
    public HexNode nextNodeInPath(HexNode goal, HexNode cur)
    {
        // Handle null cases for goal or current node
        if (goal == null || cur == null)
        {
            return null;
        }

        // Generate the flow field (pathfinding graph) for the goal node if it doesn't exist yet
        GenerateFlowField(goal);

        // Try to get the mapItem containing the predecessor node for the current node from the pathfinding graph
        mapItem nextNode;
        PathfindingGraphs[goal].TryGetValue(cur, out nextNode);

        // Return the predecessor node (the next node in the path)
        return nextNode.node;
    }

    // Function to find the closest node to a given world position (currently not very efficient)
    public HexNode findClosetNode(Vector3 v)
    {
        // Initial empty collider array
        Collider[] c = new Collider[0];

        // Variable to store the current search radius
        var i = 1f;

        // Loop until a collider is found within the radius
        while (c.Length <= 0)
        {
            // Perform an overlap box check using the current radius and a layer mask for hexagon terrain
            c = Physics.OverlapBox(v, new Vector3(i, i, i), Quaternion.identity, LayerMask.GetMask("HexagonTerrain"));

            // Increase the radius exponentially if no collider is found
            i = i + i;

            // Exit the loop if the radius becomes too large (potential infinite loop prevention)
            if (i >= 512)
            {
                return null;
            }
        }

        // Store the search radius for debugging purposes (unused in final implementation)
        debug_rad = i;
        debug = v;

        // Create a list to store potential close nodes (colliders that overlapped with the box)
        List<HexNode> list = new List<HexNode>();

        // Loop through the found colliders and try to get their HexNode components
        for (int j = 0; j < c.Length; j++)
            {
                HexNode n = null;
                c[j].gameObject.TryGetComponent<HexNode>(out n);
                if (n != null)
                {
                    list.Add(n);
                }
            }

        // Initialize variables to store the closest node and its distance
        float dist = -1;
        HexNode closest = null;

        // Loop through the list of potential close nodes
        for (int j = 0; j < list.Count; j++)
        {
            // Calculate the distance between the current node and the world position
            var temp = Vector3.Distance(list[j].Vec3Location(), v);

            // Check if the distance is lower than the current closest or if no closest has been found yet
            // Also consider a maximum terrain difficulty threshold (optional optimization)
            if ((temp < dist || dist == -1) && list[j].terrainDif <= 100)
            {
                dist = temp;
                closest = list[j];
            }
        }

        // Return the closest node found within the search radius
        return closest;
    }

    // Function to get a specific node by its grid position
    public HexNode getNode(int x, int z)
    {
        return map[x, z];
    }

    // Function to get the entire map as a 2D array of HexNode references
    public HexNode[,] getMap()
    {
        return map;
    }

    // Function to get the dimensions (width and depth) of the map as an integer array
    public int[] getMapSize()
    {
        return new int[] { map_x, map_z };
    }

    // Function to generate flow fields (pre-calculated pathfinding information) for all nodes in the map
    public void GenerateAll()
    {
        foreach (var node in map)
        {
            GenerateFlowField(node);
        }
    }

    // Function to set the type (visual representation) of a node at a specific grid position
    public void setCrystal(int x, int z)
    {
        map[x, z].type = "Hex27";
        map[x, z].terrainDif = 100;
    }

    // Function to set the terrain difficulty of a node at a specific grid position
    public void setNewDiff(int x, int z, int diff)
    {
        map[x, z].terrainDif = diff;
    }

    // Function to find the nearest crystal node (node with type "Hex27") to a given world position
    public HexNode findNearestCrystal(Vector3 v)
    {
        // Initialize variables to store the closest crystal node and its distance
        float dist = -1;
        HexNode closest = null;

        // Loop through all nodes in the map
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                var node = map[i, j];

                // Check if the current node is a crystal node
                if (node.type == "Hex27")
                {
                    // Calculate the distance between the crystal node and the world position
                    var temp = Vector3.Distance(node.Vec3Location(), v);

                    // Update closest crystal node if the distance is lower or no closest has been found yet
                    if ((temp < dist || dist == -1))
                    {
                        dist = temp;
                        closest = node;
                    }
                }
            }
        }

        // Return the closest crystal node found
        return closest;
    }

    // Unused debugging function to draw a sphere for visualization purposes
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(debug, debug_rad);
    }
}
