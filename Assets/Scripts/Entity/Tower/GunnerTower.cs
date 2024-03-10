using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunnerTower : BaseTower

{
    GunnerTower()
    {
        health = 150.0f;
        attackRange = 7.5f;
        attackSpeed = 10.0f;
        attackDamage = 1.0f;
        damageType = DamageSystem.KINETIC_ELEMENT;
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
}
