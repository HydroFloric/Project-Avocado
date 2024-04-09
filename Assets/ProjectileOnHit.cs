using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileOnHit : MonoBehaviour
{
    public GameObject originObject;
    public GameObject instantiateObject = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.transform.position = originObject.transform.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bool burning = false;
        if (other.gameObject != originObject.transform.parent.gameObject)
        {
            if(instantiateObject != null)
            {
                Collider[] colliderHits = Physics.OverlapSphere(other.transform.position, 0.25f);

                foreach (Collider collider in colliderHits)
                {
                    if(collider.gameObject.name.Equals("Flames(Clone)"))
                    {
                        burning = true;
                        collider.GetComponentInChildren<DamageOverTime>().damageRadius += 0.5f;
                    }
                }
                if(!burning) Instantiate(instantiateObject, other.transform.position, Quaternion.identity);

            }
        }
    }
}
