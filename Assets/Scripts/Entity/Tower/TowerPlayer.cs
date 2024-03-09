using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlayer : MonoBehaviour
{
   public HexNode BaseLocation;

    private void Start()
    {
        BaseLocation = GetComponentInParent<MapManager>().getNode(5, 5);
    }
}
