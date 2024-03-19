using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName = "xXJon_DoXx"; //Placeholder
    public int points = 0; //Kills for the swarm, crystals for the towers.
    public HexNode BaseLocation;
    public List<HexNode> ControlledCrystals = new List<HexNode>();

    public void SetCamera()
    {
        var temp = BaseLocation.Vec3Location();
        gameObject.transform.GetChild(0).position = new Vector3(temp.x, 120, temp.z);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
