using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GunnerTower : BaseTower

{
    private EntityBase currentTarget;
    private float TimeSinceLastShot;
    GunnerTower()
    {
        health = 150.0f;
        maxRange = 7.5f;
        RateOfFire = 10.0f;
        attackDamage = 1.0f;
        damageType = DamageSystem.KINETIC_ELEMENT;
        damageResist = DamageSystem.KINETIC_ELEMENT;
    }

    private void FixedUpdate()
    {
        TimeSinceLastShot += Time.deltaTime;
        float distance = float.MaxValue;
        if(currentTarget != null)
        {
            distance = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);
        }
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position,maxRange, LayerMask.GetMask("Swarm"));
        
        foreach (Collider hit in hits)
        {
          if(Vector3.Distance(hit.gameObject.transform.position,gameObject.transform.position) < distance)
            {
                currentTarget = hit.gameObject.GetComponent<EntityBase>();
            }
        }
        if(TimeSinceLastShot >= 1/RateOfFire && currentTarget != null)
        {
            Attack(currentTarget, (int)attackDamage, maxRange);
            TimeSinceLastShot = 0;
        }
    }

    private void Attack(EntityBase target, int dmg, float range)
    {
        float damageMultiplier = DamageSystem.DamageFactor(target, this.damageType);
        target.health -= dmg * damageMultiplier;
        Debug.DrawRay(gameObject.transform.position, target.transform.position - gameObject.transform.position , Color.yellow, 1f);

        /*
        if (Vector3.Distance(this.toVec3(), target.toVec3()) < range && target != null) //if target is within range of attacker.
        {
            float damageMultiplier = DamageSystem.DamageFactor(target, this.damageType);
            target.health -= dmg * damageMultiplier;
            Debug.DrawRay(gameObject.transform.position, target.transform.position, Color.yellow, 0.2f);
        }
        */
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, maxRange);
    }
}
