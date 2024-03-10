using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlayer : MonoBehaviour
{
   public HexNode BaseLocation;
   public List<BaseTower> towers = new List<BaseTower>();
   public List<HexNode> ControlledCrystals = new List<HexNode>();

   public int TowerLimit = 5; //some multiple of crystals controlled or smth;
    private void Start()
    {
        BaseLocation = GetComponentInParent<MapManager>().getNode(5, 5);
    }

    public bool AddTower(GameObject e, HexNode Location)
    {

        if(towers.Count < TowerLimit && Physics.OverlapSphere(Location.Vec3Location(), 0.5f, LayerMask.GetMask("Tower")).Length <= 0)
        {
            var temp = Instantiate(e);

            temp.GetComponent<BaseTower>().currentLocation = Location;
            temp.transform.position = Location.Vec3Location();
            towers.Add(temp.GetComponent<BaseTower>());
            return true;
        }
        return false;
    }
}
