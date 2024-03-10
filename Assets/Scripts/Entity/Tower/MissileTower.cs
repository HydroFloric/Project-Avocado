using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : BaseTower

{
    MissileTower()
    {
        health = 150.0f;
        attackRange = 10.0f;
        attackSpeed = 1.0f;
        attackDamage = 30.0f;
        damageType = DamageSystem.EXPOSIVE_ELEMENT;
        damageResist = DamageSystem.EXPOSIVE_ELEMENT;
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
