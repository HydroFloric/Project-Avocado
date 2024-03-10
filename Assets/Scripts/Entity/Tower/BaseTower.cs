using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : EntityBase
{
    public float RateOfFire = 1.0f;
    public float maxRange = 1.0f;

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
        float distance = float.MaxValue;
        if (currentTarget != null)
        {
            distance = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);
        }
        Collider[] colliderHits = Physics.OverlapSphere(gameObject.transform.position, maxRange, LayerMask.GetMask("Swarm"));

        foreach (Collider hits in colliderHits)
        {
            if (Vector3.Distance(hits.gameObject.transform.position, gameObject.transform.position) < distance)
            {
                RaycastHit hitState;
                if (hits.Raycast(new Ray(gameObject.transform.Find("GunPos").position, hits.gameObject.transform.position - gameObject.transform.Find("GunPos").position), out hitState, maxRange))
                    //If there is a direct path from the tower to the target.
                {
                    currentTarget = hits.gameObject.GetComponent<EntityBase>();
                }
            }
        }
        if (TimeSinceLastShot >= 1 / RateOfFire && currentTarget != null)
        {
            Attack(currentTarget, (int)attackDamage, maxRange);
            TimeSinceLastShot = 0;
        }
    }

    private void Attack(EntityBase target, int dmg, float range)
    {
        DamageSystem.DealDamage(target, this);
        Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, maxRange);
    }
}
