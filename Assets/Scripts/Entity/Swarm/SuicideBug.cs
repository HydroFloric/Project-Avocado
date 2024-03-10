using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideBug : BaseBug
{
    SuicideBug()
    {
        health = 50.0f;
        speed = 0.75f;
        maxRange = 1.0f;
        attackDamage = 100.0f;
        damageType = DamageSystem.EXPOSIVE_ELEMENT;
        damageResist = DamageSystem.KINETIC_ELEMENT;
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
