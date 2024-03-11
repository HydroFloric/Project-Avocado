using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTower : BaseTower

{
    public GameObject flameEffect;
    FlameTower()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 150.0f;
        attackRange = 5.0f;
        attackSpeed = 2f;
        attackDamage = 5.0f;
        damageType = DamageSystem.MAGIC_ELEMENT;
        damageResist = DamageSystem.MAGIC_ELEMENT;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        Vector3 vector3 = target.transform.position;
        vector3.y -= 0.25f;
        Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        Instantiate(flameEffect, vector3, Quaternion.identity);
    }
}
