using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject projectile;
    public float shotSpeed;
    private Vector3 dir;


    // Start is called before the first frame update
    void Start()
    {
        dir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector3 launchVector)
    {
        dir = launchVector * shotSpeed;
        projectile.GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
    }

}
