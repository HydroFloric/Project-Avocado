using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBug : EntityBase
{
    private EntityBase currentTarget;
    private float TimeSinceLastAttack;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        TimeSinceLastAttack += Time.deltaTime;
        float targetDistance = float.MaxValue;
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);
            if(targetDistance > attackRange) 
            {
                currentTarget = null;
            }
        }
        else
        {
            targetDistance = float.MaxValue;
        }
        Collider[] colliderHits = Physics.OverlapSphere(gameObject.transform.position, attackRange, LayerMask.GetMask("Tower"));

        foreach (Collider hits in colliderHits)
        {
            if (Vector3.Distance(hits.gameObject.transform.position, gameObject.transform.position) < attackRange)
            {
                RaycastHit hitState;
                Ray ray = new Ray(gameObject.transform.position, Vector3.Normalize(hits.gameObject.transform.position - gameObject.transform.position));
                if (hits.Raycast(ray, out hitState, attackRange))
                //If there is a direct path from the tower to the target.
                {
                    currentTarget = hits.gameObject.GetComponent<EntityBase>();
                }
            }
        }
        if (TimeSinceLastAttack >= 1 / attackSpeed && currentTarget != null)
        {
            Attack(currentTarget, attackDamage, attackRange);
            TimeSinceLastAttack = 0;
        }
    }

    private void Attack(EntityBase target, float dmg, float range)
    {
        DamageSystem.DealDamage(target, this);
        Debug.DrawRay(gameObject.transform.position, target.transform.position - gameObject.transform.position, Color.blue, 0.2f);
    }
}
