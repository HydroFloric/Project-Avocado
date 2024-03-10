using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    //Damage type constants. Apply these to the damage type and resistance variables of the entity.
    public const int NO_ELEMENT = -1;
    public const int MAGIC_ELEMENT = 1;
    public const int KINETIC_ELEMENT = 2;
    public const int EXPOSIVE_ELEMENT = 3;

    EntityBase[] entityInitializer;
    List<EntityBase> entities = new List<EntityBase>();

    // Start is called before the first frame update
    void Start()
    {
        //Initialize entities list with all current entities loaded.
        //But only if they're not already present.
        entityInitializer = FindObjectsByType<EntityBase>(FindObjectsSortMode.InstanceID);
        foreach (EntityBase entity in entityInitializer)
        {
            AddEntity(entity);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        entityInitializer = FindObjectsByType<EntityBase>(FindObjectsSortMode.InstanceID);
        foreach (EntityBase entity in entityInitializer)
        {
            AddEntity(entity);
            if (entity == null)
            {
                entities.Remove(entity);
            }
        }

       //Check through the entities list and do what must be done.
        foreach (EntityBase entity in entities)
        {
            if(entity.health <= 0 && entity != null)
            {
                Destroy(entity.gameObject);
            }
        }
    }

    public void AddEntity(EntityBase entity)
    {
        //Add entity to damageSystem's entity list but only if it isn't already there, duh.
        if (!entities.Contains(entity))
        {
            entities.Add(entity);
        }
    }

    public static float DamageFactor(EntityBase entity, int attackType)
    {
        //Returns a float representing the multiplication factor to be applied to the damage of an attack.
        //Returned factor is based off the attack type vs the damage resistance of the target.
        //0.5 == half damage, 2.0 == double damage, 1.0 == base damage.
        switch (attackType)
        {
            case KINETIC_ELEMENT:
                if (entity.damageResist == KINETIC_ELEMENT) return 0.5f;
                else if (entity.damageResist == MAGIC_ELEMENT) return 2.0f;
                else if (entity.damageResist == EXPOSIVE_ELEMENT) return 1.0f;
                break;
            case EXPOSIVE_ELEMENT:
                if (entity.damageResist == KINETIC_ELEMENT) return 2.0f;
                else if (entity.damageResist == MAGIC_ELEMENT) return 1.0f;
                else if (entity.damageResist == EXPOSIVE_ELEMENT) return 0.5f;
                break;
            case MAGIC_ELEMENT:
                if (entity.damageResist == KINETIC_ELEMENT) return 1.0f;
                else if (entity.damageResist == MAGIC_ELEMENT) return 0.5f;
                else if (entity.damageResist == EXPOSIVE_ELEMENT) return 2.0f;
                break;
            default:
                return 0;
        }
        return 0;
    }

    public static float DealDamage(EntityBase target, EntityBase attacker)
    {
        float damageMult = DamageFactor(target, attacker.damageType); //Get damage resistance or weakness factor
        target.health -= attacker.attackDamage * damageMult;
        return damageMult * attacker.attackDamage; //Return damage dealt because we'll probably need this later.
    }
}
