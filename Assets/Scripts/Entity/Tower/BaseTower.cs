using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTower : EntityBase

{
    public bool active = true;
    public Pipe connectionToBase;

    public float RateOfFire = 1.0f;
    public float maxRange = 1.0f;
    public float attackDamage = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.0f; //Towers *probably* shouldn't have a move speed.
    }

    // Update is called once per frame
    void Update()
    {
        active = connectionToBase.active;
    }
    public void init(Pipe p, HexNode l)
    {
        base.init(l);
        connectionToBase = p;
        active = p.active;
    }
    private void Attack(EntityBase target, int dmg, float range)
    {
        if (active)
        {
            if (Vector3.Distance(this.toVec3(), target.toVec3()) < range && target != null) //if target is within range of attacker.
            {
                float damageMultiplier = DamageSystem.DamageFactor(target, this.damageType);
                target.health -= dmg * damageMultiplier;
            }
        }
        }

    }
