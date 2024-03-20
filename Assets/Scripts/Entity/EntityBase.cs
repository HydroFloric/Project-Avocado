using UnityEngine;

/* Try not to actually do any work in this class just add attributes that entities have
 * (spd, resistances, damage)
 */
public enum State
{
    idle,
    attacking,
    moving
}
public class EntityBase : MonoBehaviour
{
    //Storing this since transformer my not be in existance...
    public float x, y, z;
    public float baseRotation; //degree that a model is rotated on the x axis to appear correct.

    public State state = State.idle;
    public bool active = false;

    public float maxHealth = 100;
    public float health = 100.0f;
    public float speed = 1.0f;
    public float cost = 100;

    public float attackDamage = 1.0f;
    public float attackSpeed = 1.0f;
    public float attackRange = 1.0f;
    public int damageResist = DamageSystem.NO_ELEMENT;
    public int damageType = DamageSystem.NO_ELEMENT;

    private EntityBase currentTarget;
    private float TimeSinceLastAttack;

    public HexNode currentLocation;
    public HexNode pathingTo;
    
    //these are alledgely bad practice, I wont remove em just incase they are being used!
    public EntityBase() 
    {
        x = 0;
        y = 0;
        z = 0;
    }
    public EntityBase(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z; 
    }

    public EntityBase(float _x, float _y, float _z, int dmgResist, int dmgType, int hp, float spd)
    {
        x = _x;
        y = _y;
        z = _z;

        damageResist = dmgResist;
        damageType = dmgType;
        health = hp;
        speed = spd;
    }

    public void init(HexNode l)
    {
        currentLocation = l;
    }
    public Vector3 toVec3()
    {
        
        return new Vector3(x, y, z);
    }
    public void SetVec3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void targetCheck(string exclusionLayer, string targetLayer, EntityBase callerID)
    {
        attackDamage = callerID.attackDamage;
        attackRange = callerID.attackRange;
        attackSpeed = callerID.attackSpeed;

        TimeSinceLastAttack += Time.deltaTime;
        float targetDistance = float.MaxValue;
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);
            if (targetDistance > attackRange)
            {
                currentTarget = null;
            }
        }
        else
        {
            targetDistance = float.MaxValue;
        }
        LayerMask mask = LayerMask.GetMask(exclusionLayer, "Ignore Raycast");
        mask = ~mask;
        Collider[] colliderHits = Physics.OverlapSphere(gameObject.transform.position, attackRange, LayerMask.GetMask(targetLayer));

        currentTarget = null; //This might change, but for the time being this aids with LOS targetting
        foreach (Collider hits in colliderHits)
        {
            if (Vector3.Distance(hits.gameObject.transform.position, gameObject.transform.position) < attackRange)
            {
                RaycastHit hitState;
                Ray ray = new Ray(gameObject.transform.Find("GunPos").position, Vector3.Normalize(hits.gameObject.transform.position - gameObject.transform.Find("GunPos").position));
                Debug.DrawRay(gameObject.transform.position, hits.transform.position - gameObject.transform.position, Color.blue, 0.2f);
                if (Physics.Raycast(ray, out hitState, attackRange, mask))
                //Is there a direct path from the attacker to the target.
                {
                    if (hitState.collider.Equals(hits))
                    {
                        currentTarget = hits.gameObject.GetComponent<EntityBase>();
                    }
                }
            }
        }
        if (TimeSinceLastAttack >= 1 / attackSpeed && currentTarget != null)
        {
            callerID.TryAttack(currentTarget, attackDamage, attackRange);
            TimeSinceLastAttack = 0;
        }
    }

    public virtual void Attack(EntityBase target, float dmg, float range)
    {
        DamageSystem.DealDamage(target, this);
        //Debug.DrawRay(gameObject.transform.Find("GunPos").position, target.transform.position - gameObject.transform.Find("GunPos").position, Color.red, 0.2f);
    }
    public virtual void TryAttack(EntityBase target, float damage, float range)
    {
        if (active)
        {
            Attack(target, damage, range);
        }
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
