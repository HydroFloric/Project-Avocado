using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class GunnerTower : BaseTower
{
    private GameObject gunPos;
    GunnerTower()
    {
        
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        health = 150.0f;
        attackRange = 7.5f;
        attackSpeed = 10.0f;
        attackDamage = 1.0f;
        damageType = DamageSystem.KINETIC_ELEMENT;
        damageResist = DamageSystem.KINETIC_ELEMENT;
        gunPos = gameObject.transform.Find("GunPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack(EntityBase target, float dmg, float range)
    {
        base.Attack(target, dmg, range);
        GameObject shell = gameObject.transform.Find("CannonShell").gameObject;
        Vector3 vector3 = target.transform.position;
        Vector3 launchVector = target.transform.position - gunPos.transform.position;
        //Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
        shell.transform.position = gunPos.transform.position;
        shell.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        Quaternion shellDir = Quaternion.FromToRotation(Vector3.up, launchVector);
        shell.transform.rotation = shellDir;
        gunPos.GetComponent<LaunchProjectile>().Launch(launchVector);


        ParticleSystem[] muzzles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem muzzle in muzzles)
        {
            muzzle.Play();
        }
    }
}
