using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerNetwork : NetworkBehaviour
{
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);
    private void Update()
    {
        //Debug.Log(OwnerClientId + "; random number : " + randomNumber.Value);

        if (!IsOwner) return;
        Vector3 movDir = new Vector3(0, 0, 0);
        //if (Input.GetKey(KeyCode.T))
        //{
        //    randomNumber.Value = Random.Range(0, 100);
        //}

        if (Input.GetKey(KeyCode.W)) movDir.z = +1f;
        if (Input.GetKey(KeyCode.A)) movDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) movDir.x = +1f;
        if (Input.GetKey(KeyCode.S)) movDir.z = -1f;

        float moveSpeed = 3f;
        transform.position += movDir * moveSpeed * Time.deltaTime;

    }
}
