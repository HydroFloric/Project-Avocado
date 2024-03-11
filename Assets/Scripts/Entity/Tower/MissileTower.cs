using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : BaseTower

{
    private float explosionRange;
    public GameObject explosionEffect;
    MissileTower()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 150.0f;
        attackRange = 10.0f;
        attackSpeed = 0.5f;
        attackDamage = 30.0f;
        explosionRange = 1.5f;
        damageType = DamageSystem.EXPOSIVE_ELEMENT;
        damageResist = DamageSystem.EXPOSIVE_ELEMENT;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        Collider[] colliderHits = Physics.OverlapSphere(target.transform.position, explosionRange, LayerMask.GetMask("Swarm"));

        foreach (Collider collider in colliderHits)
        {
            DamageSystem.DealDamage(collider.gameObject.GetComponent<EntityBase>(), this);
            Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        }
        Instantiate(explosionEffect, target.transform.position, Quaternion.identity);
    }
}
