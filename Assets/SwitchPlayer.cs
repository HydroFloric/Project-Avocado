using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPlayer : MonoBehaviour
{
    public int defaultPlayer = 0;
    public int currentPlayer = 0;
    void Start()
    {
        if (defaultPlayer == 0)
        {
            GetComponentInChildren<TowerPlayer>().enabled = true;
        }
        if(defaultPlayer== 1)
        {
            GetComponentInChildren<SwarmPlayer>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentPlayer= currentPlayer + 1;
        }
        if(currentPlayer > 1)
        {
            currentPlayer = defaultPlayer;
        }
        if (currentPlayer == 0)
        {
            GetComponentInChildren<TowerPlayer>().enabled = true;
            GetComponentInChildren<SwarmPlayer>().enabled = false;
        }
        if (currentPlayer == 1)
        {
            GetComponentInChildren<TowerPlayer>().enabled = false;
            GetComponentInChildren<SwarmPlayer>().enabled = true;
        }
    }
}
