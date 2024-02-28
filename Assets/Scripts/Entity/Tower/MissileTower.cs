using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : BaseTower

{
    MissileTower()
    {
        health = 150.0f;
        maxRange = 10.0f;
        RateOfFire = 1.0f;
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

    private void Attack(EntityBase target, int dmg, float range)
    {
        if(Vector3.Distance(this.toVec3(), target.toVec3()) < range && target != null) //if target is within range of attacker.
        {
            float damageMultiplier = DamageSystem.DamageFactor(target, this.damageType);
            target.health -= dmg * damageMultiplier;
        }
    }
}
