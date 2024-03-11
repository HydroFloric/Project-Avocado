using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseTower : EntityBase
{
    public Pipe connectionToBase;
    private EntityBase currentTarget = null;
    private float TimeSinceLastShot;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.0f; //Towers *probably* shouldn't have a move speed.
    }

    public void init(Pipe p, HexNode l)
    {
        base.init(l);
        connectionToBase = p;
        active = p.active;
    }

    private void FixedUpdate()
    {
        targetCheck("Tower", "Swarm", this);

        if (connectionToBase != null)
        {
            active = connectionToBase.active;
            if (active == true)
            {
                foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    r.material.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
            }
            if (active == false)
            {
                foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    r.material.color = new Color(0,0,0,0.5f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);
    }
}
