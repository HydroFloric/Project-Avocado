using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class destroySelf : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = gameObject.GetComponentInChildren<ParticleSystem>().main.duration;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if( timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
