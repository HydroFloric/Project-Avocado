using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmCrystal : MonoExec
{
    //exec is invoked here when Swarm is controlling crystal
    //this will handle tick based spawning of entites
    public int type; //which index in the entity manager will be called
    public HexNode spawnTile;
    public HexNode spawnWaypoint; //this will default to spawnTile
    

    public override void exec()
    {

    }
}
