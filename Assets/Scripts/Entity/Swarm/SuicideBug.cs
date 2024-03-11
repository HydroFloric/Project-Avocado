using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideBug : BaseBug
{
    public GameObject explosionEffect;
    private float explosionRange;
    SuicideBug()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 50.0f;
        speed = 0.75f;
        attackRange = 1.5f;
        attackSpeed = 0.25f;
        attackDamage = 100.0f;
        explosionRange = attackRange;
        damageType = DamageSystem.EXPOSIVE_ELEMENT;
        damageResist = DamageSystem.KINETIC_ELEMENT;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        Collider[] colliderHits = Physics.OverlapSphere(gameObject.transform.position, explosionRange, LayerMask.GetMask("Tower"));

        foreach (Collider collider in colliderHits)
        {
            DamageSystem.DealDamage(collider.gameObject.GetComponent<EntityBase>(), this);
            Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        }
        Instantiate(explosionEffect, this.transform.position, Quaternion.identity);
        this.health = 0; //Dies
    }

    public override void OnDeath()
    {
        Attack(this, attackDamage, explosionRange);
        base.OnDeath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, explosionRange);
    }
}
