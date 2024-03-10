using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
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
            var child = Instantiate(pipeGO);
            child.transform.position = player.BaseLocation.Vec3Location();
            var tempPipe = child.AddComponent<Pipe>();
            tempPipe.initPipe(null, player.BaseLocation);
            pipes.Add(tempPipe);
            root = tempPipe;
            curPipe = root;

        }
        else
        {
            float dist = -1;
            curPipe = root;
            foreach (var p in pipes)
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
            if (timeSinceLast >= pipeSpawnTickRate) buildPipe();
            timeSinceLast += Time.deltaTime;
        }
    }
    void buildPipe()
    {
        timeSinceLast = 0f;
        var node = mapManager.nextNodeInPath(currentGoal, curPipe.location);
        if (currentGoal == node)
        {
            curPipe.connectedToCrystal = true;
            currentGoal = null;
            curPipe = null;

            return;
        }
        var child = Instantiate(pipeGO);
        child.transform.position = node.Vec3Location();
        var tempPipe = child.AddComponent<Pipe>();
        tempPipe.initPipe(curPipe, node);
        pipes.Add(tempPipe);
        curPipe.addChild(tempPipe);
        curPipe = tempPipe;

    }
    public bool contains(Vector3 v)
    {
        Collider[] hits = Physics.OverlapSphere(v, 0.5f, LayerMask.GetMask("Pipe"));
        if (hits.Length == 0) return false;
        else return true;
    }
}

class Pipe : MonoBehaviour
{
    Pipe Parent;
    public List<Pipe> Children = new List<Pipe>();
    public HexNode location;
    public bool active = false;

    public bool connectedToCrystal = false;

    public void initPipe(Pipe parent, HexNode location)
    {
        Parent = parent;
        this.location = location;

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
