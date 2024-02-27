using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalBase : MonoBehaviour
{

    public int controledBy; //0 none, 1 swarm, 2 tower

    public MonoExec selectedScript; //you know i hope anyone reading this has just come to expect jank over readability

    public void FixedUpdate()
    {
        if (controledBy== 0)
        {
            return;
        }
        if(controledBy == 1)
        {
            selectedScript = gameObject.GetComponent<SwarmCrystal>();
            
        }
        if(controledBy == 2)
        {
            selectedScript = gameObject.GetComponent<TowerCrystal>();
        }
        selectedScript.exec();
    }
}
