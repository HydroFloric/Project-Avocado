using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FlameTower : BaseTower

{
    private GameObject gunPos;
    public GameObject flameEffect;
    FlameTower()
    {
        
    }

    // Start is called before the first frame update
    new void Start()
    {
        health = 150.0f;
        attackRange = 5.0f;
        attackSpeed = 1f;
        attackDamage = 5.0f;
        damageType = DamageSystem.MAGIC_ELEMENT;
        damageResist = DamageSystem.MAGIC_ELEMENT;
        gunPos = gameObject.transform.Find("GunPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        Vector3 vector3 = target.transform.position;
        Vector3 launchVector = target.transform.position - gunPos.transform.position;
        //Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        //Instantiate(flameEffect, vector3, Quaternion.identity);
        GameObject flames = gameObject.transform.Find("FireGlob").gameObject;
        flames.transform.position = gunPos.transform.position;
        flames.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gunPos.GetComponent<LaunchProjectile>().Launch(launchVector);

        ParticleSystem[] muzzles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem muzzle in muzzles)
        {
            muzzle.Play();
        }
    }
}
