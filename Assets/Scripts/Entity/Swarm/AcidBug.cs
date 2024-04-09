using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcidBug : BaseBug
{
    public GameObject acidEffect;
    public Animation ani;
    private GameObject gunPos;
    AcidBug() 
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        health = 25.0f;
        speed = 0.5f;
        attackRange = 5.0f;
        attackSpeed = 0.35f;
        attackDamage = 1.0f;
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
        RaycastHit hitState;
        //Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        Ray ray = new Ray(gunPos.transform.position, Vector3.Normalize(target.gameObject.transform.position - gunPos.transform.position));
        Physics.Raycast(ray, out hitState, attackRange, LayerMask.GetMask("Tower"));
        //Instantiate(acidEffect, vector3, Quaternion.Euler(hitState.normal));
        GameObject acid = gameObject.transform.Find("AcidGlob").gameObject;
        acid.transform.position = gunPos.transform.position;
        acid.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gunPos.GetComponent<LaunchProjectile>().Launch(launchVector);

        ParticleSystem[] muzzles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem muzzle in muzzles)
        {
            muzzle.Play();
        }
    }
}
