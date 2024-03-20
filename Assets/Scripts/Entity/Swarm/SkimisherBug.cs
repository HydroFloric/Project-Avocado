using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkimisherBug : BaseBug
{
    SkimisherBug()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 45.0f;
        speed = 1.25f;
        attackRange = 1.5f;
        attackSpeed = 1.0f;
        attackDamage = 15.0f;
        damageType = DamageSystem.KINETIC_ELEMENT;
        damageResist = DamageSystem.EXPOSIVE_ELEMENT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
