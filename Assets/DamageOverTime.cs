using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageOverTime : MonoBehaviour
{
    private float timer = 1.0f;
    private float damageTick;
    public float damageRadius;
    public string layerTarget;
    public EntityBase hazardOwner;
    // Start is called before the first frame update

    DamageOverTime(float dmgT, float radius, string targetType, EntityBase creator)
    {
        //damageTick = dmgT;
        //damageRadius = radius;
        //layerTarget = targetType;
        //hazardOwner = creator;
    }
    void Start()
    {
        timer = damageTick;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            timer = 1.0f;
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, damageRadius, LayerMask.GetMask(layerTarget));

            foreach (Collider collider in colliderHits)
            {
                DamageSystem.DealDamage(collider.gameObject.GetComponent<EntityBase>(), hazardOwner);
                Debug.DrawRay(gameObject.transform.position, collider.gameObject.GetComponent<EntityBase>().transform.position - gameObject.transform.position, Color.yellow, 0.2f);
            }
        }
        
    }
}
