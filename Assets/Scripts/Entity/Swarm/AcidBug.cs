using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidBug : BaseBug
{
    AcidBug() 
    {
        health = 25.0f;
        speed = 0.5f;
        attackRange = 5.0f;
        attackSpeed = 1.0f;
        attackDamage = 5.0f;
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
}
