using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkimisherBug : BaseBug
{
    SkimisherBug()
    {
        health = 75.0f;
        speed = 1.25f;
        maxRange = 1.0f;
        attackDamage = 15.0f;
        damageType = DamageSystem.KINETIC_ELEMENT;
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

    private void Attack(EntityBase target, int dmg, float range)
    {
        if (Vector3.Distance(this.toVec3(), target.toVec3()) < range && target != null) //if target is within range of attacker.
        {
            float damageMultiplier = DamageSystem.DamageFactor(target, this.damageType);
            target.health -= dmg * damageMultiplier;
        }
    }
}
