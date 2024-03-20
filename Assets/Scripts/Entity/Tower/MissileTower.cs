using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : BaseTower

{
    private GameObject gunPos;
    //private float explosionRange;
    public GameObject explosionEffect;
    MissileTower()
    {
        
    }

    // Start is called before the first frame update
    new void Start()
    {
        health = 150.0f;
        attackRange = 10.0f;
        attackSpeed = 0.25f;
        attackDamage = 30.0f;
        //explosionRange = 1.5f;
        damageType = DamageSystem.EXPOSIVE_ELEMENT;
        damageResist = DamageSystem.EXPOSIVE_ELEMENT;
        gunPos = gameObject.transform.Find("GunPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        GameObject shell = gameObject.transform.Find("CannonShell").gameObject;
        Vector3 vector3 = target.transform.position;
        Vector3 launchVector = target.transform.position - gunPos.transform.position;
        
        shell.transform.position = gunPos.transform.position;
        shell.GetComponent<Rigidbody>().velocity = Vector3.zero;

        Quaternion shellDir = Quaternion.FromToRotation(Vector3.up, launchVector);
        shell.transform.rotation = shellDir;
        gunPos.GetComponent<LaunchProjectile>().Launch(launchVector);
        //Instantiate(explosionEffect, target.transform.position, Quaternion.identity);

        ParticleSystem[] muzzles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem muzzle in muzzles)
        {
            muzzle.Play();
        }
    }
}
