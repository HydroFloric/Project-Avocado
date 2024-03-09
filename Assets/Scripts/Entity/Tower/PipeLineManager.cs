using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class PipeLineManager : MonoBehaviour
{
    MapManager mapManager;
    TowerPlayer player;


    Pipe root;
    List<Pipe> pipes = new List<Pipe>();
    Pipe curPipe;
    HexNode currentGoal;

    float pipeSpawnTickRate = 0.1f;
    float timeSinceLast = 0f;
    public GameObject pipeGO;

    private void Start()
    {
        mapManager = GetComponentInParent<MapManager>();
        player = GetComponent<TowerPlayer>();
    }
    //I might need to grab my data structures textbook if i do anymore of this wacky shit!
    public void setGoal(Vector2 a)
    {
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(a.x, a.y, Camera.main.transform.position.z));

        HexNode goal = mapManager.findNearestCrystal(origin);

        if (root == null)
        {

            root = new Pipe(null, player.BaseLocation, null);
            pipes.Add(root);
            curPipe = root;
        }
        else
        {
            float dist = -1;
            curPipe = root;
            foreach(var p in pipes)
            {
                var temp = Vector3.Distance(p.location.Vec3Location(), goal.Vec3Location());
                if ((temp < dist || dist == -1)) { dist = temp; curPipe = p; }
            }



            /*
            //okay so this runs if your goal is set and a tree already exists. it will traverse the tree
            //looking for the the first node that doesnt exist in the tree. 
            var timeout = 0;
            var pipe = root;
            while(pipe != null || timeout > 100)
            {
                timeout++;
                var node = mapManager.nextNodeInPath(goal, pipe.location);
                foreach(var child in pipe.Children)
                {
                    if(child.location != node)
                    {
                        curPipe = pipe;
                        pipe = null;
                        continue;
                    }
                    pipe = child;
                }
            }
            */
        }

        currentGoal = goal;
        
    }
    public void FixedUpdate()
    {

        if (curPipe != null && currentGoal != null)
        {
            if (timeSinceLast >= pipeSpawnTickRate)
            {
                var node = mapManager.nextNodeInPath(currentGoal, curPipe.location);
                if (currentGoal == node)
                {
                    currentGoal = null;
                    curPipe = null;
                    timeSinceLast = 0f;
                    return;
                }
                
                var child = Instantiate(pipeGO);
                child.transform.position = node.Vec3Location() + Vector3.up;
                var tempPipe = new Pipe(curPipe, node, child);
                pipes.Add(tempPipe);
                curPipe.addChild(tempPipe);
                curPipe = tempPipe;
               
               
            }
            timeSinceLast += Time.deltaTime;
        }
    }
}

class Pipe
{
    Pipe Parent;
    GameObject reference;
    public List<Pipe> Children = new List<Pipe>();
    public HexNode location;
    bool active = false;
    
    public Pipe(Pipe parent, HexNode location, GameObject reference)
    {
        Parent = parent;
        this.location = location;
        this.reference = reference;

    }
    //You can imagine the network of pipes like a tree data structure, all pipes can only have one parent but can have any number of children.
    //when a pipe is destoryed all the children pipes are effected
    public void setActive(bool a)
    {
        active = a;
        if (Children.Count > 0)
        {
            foreach (var child in Children)
            {
                child.setActive(a);
            }
        }
        return;
    }
    //im wondering if i should go full encapulation and just have this create the child instead of just adding it to itself.
    public void addChild(Pipe child)
    {
        Children.Add(child);
    }
}
