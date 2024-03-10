using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseTower : EntityBase
{
    private EntityBase currentTarget;
    private float TimeSinceLastShot;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.0f; //Towers *probably* shouldn't have a move speed.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        TimeSinceLastShot += Time.deltaTime;
        float targetDistance = float.MaxValue;
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);
            if (targetDistance > attackRange)
            {
                currentTarget = null;
            }
        }
        else
        {
            targetDistance = float.MaxValue;
        }
        Collider[] colliderHits = Physics.OverlapSphere(gameObject.transform.position, attackRange, LayerMask.GetMask("Swarm"));

        foreach (Collider hits in colliderHits)
        {
            if (Vector3.Distance(hits.gameObject.transform.position, gameObject.transform.position) <= attackRange)
            {
                RaycastHit hitState;
                Ray ray = new Ray(gameObject.transform.Find("GunPos").position, Vector3.Normalize(hits.gameObject.transform.position - gameObject.transform.Find("GunPos").position));
                if (hits.Raycast(ray, out hitState, attackRange))
                    //If there is a direct path from the tower to the target.
                {
                    currentTarget = hits.gameObject.GetComponent<EntityBase>();
                }
            }
        }
        if (TimeSinceLastShot >= 1 / attackSpeed && currentTarget != null)
        {
            Attack(currentTarget, attackDamage, attackRange);
            TimeSinceLastShot = 0;
        }
    }

    private void Attack(EntityBase target, float dmg, float range)
    {
        DamageSystem.DealDamage(target, this);
        Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);
    }
}
