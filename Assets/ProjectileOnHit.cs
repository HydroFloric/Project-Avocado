using System.Collections;
using System.Collections.Generic;
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
        if (other.gameObject != originObject.transform.parent.gameObject)
        {
            if(instantiateObject != null)
            {
                Instantiate(instantiateObject, other.transform.position, Quaternion.identity);
            }
            gameObject.transform.position = originObject.transform.position;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
