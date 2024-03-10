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

    public bool AddTower(GameObject e, HexNode Location, Pipe Connection)
    {
        var test1 = Physics.OverlapSphere(Location.Vec3Location(), 0.1f, LayerMask.GetMask("Tower"));
        var test2 = Physics.OverlapSphere(Location.Vec3Location(), 0.5f, LayerMask.GetMask("MapObjects"));
        if (test1.Length > 0 || test2.Length > 0) return false;

        if(towers.Count < TowerLimit)
        {
            var temp = Instantiate(e);
           
            temp.GetComponent<BaseTower>().init(Connection, Location);
            temp.transform.position = Location.Vec3Location();
            towers.Add(temp.GetComponent<BaseTower>());
            return true;
        }
        return false;
    }
}
