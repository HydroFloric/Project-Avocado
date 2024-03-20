using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Explode : MonoBehaviour
{
    public float explosionRange;
    public EntityBase explosionParent;
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliderHits = Physics.OverlapSphere(transform.position, explosionRange, LayerMask.GetMask("Swarm"));

        foreach (Collider collider in colliderHits)
        {
            DamageSystem.DealDamage(collider.gameObject.GetComponent<EntityBase>(), explosionParent);
            //Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
