using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerPlayer : Player
{
   public List<BaseTower> towers = new List<BaseTower>();

   public int TowerLimit = 5; //some multiple of crystals controlled or smth;
    TowerUI UIref;
    private void Start()
    {
        UIref = GetComponent<TowerUI>();
        if (UIref.enabled)
        {
            GetComponent<UIDocument>().enabled = true;
        }
    }
    private void OnEnable()
    {
        GetComponent<UIDocument>().enabled = true;
        GetComponent<TowerInput>().enabled = true;
        GetComponent<TowerUI>().enabled = true;
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<CameraMovement>().enabled = true;
    }
    private void OnDisable()
    {
        GetComponent<UIDocument>().enabled = false;
        GetComponent<TowerInput>().enabled = false;
        GetComponent<TowerUI>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<CameraMovement>().enabled = false;
    }
    public bool AddTower(GameObject e, HexNode Location, Pipe Connection)
    {
        var towerOverlapTest = Physics.OverlapSphere(Location.Vec3Location(), 0.1f, LayerMask.GetMask("Tower"));
        var mapObjectOverlapTest = Physics.OverlapSphere(Location.Vec3Location(), 0.5f, LayerMask.GetMask("MapObjects"));
        if (towerOverlapTest.Length > 0 || mapObjectOverlapTest.Length > 0) return false;

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
