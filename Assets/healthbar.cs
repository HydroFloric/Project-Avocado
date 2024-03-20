using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthbar : MonoBehaviour
{
    EntityBase entity;
    public GameObject redbar;
    private void Start()
    {
        entity = GetComponentInParent<EntityBase>();
        redbar = gameObject.transform.Find("hp").gameObject;
    }
    void Update()
    {
        if (entity != null)
        {
            var tempTransform = redbar.transform.localScale;

            tempTransform.z = 1 * (entity.health / (float)entity.maxHealth);
            redbar.transform.localScale = tempTransform;
        }
    }
}
