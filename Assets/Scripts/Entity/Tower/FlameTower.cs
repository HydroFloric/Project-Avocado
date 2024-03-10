using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTower : BaseTower

{
    FlameTower()
    {
        health = 150.0f;
        maxRange = 5.0f;
        RateOfFire = 5.0f;
        attackDamage = 2.0f;
        damageType = DamageSystem.MAGIC_ELEMENT;
        damageResist = DamageSystem.MAGIC_ELEMENT;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Attack(EntityBase target, int dmg, float range)
    {
        
    }
}
