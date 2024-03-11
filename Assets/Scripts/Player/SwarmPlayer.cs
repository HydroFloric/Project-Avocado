
using UnityEngine;

public class SwarmPlayer : Player
{

    private void OnEnable()
    {
        GetComponent<SwarmInput>().enabled = true;
        GetComponent<SwarmUI>().enabled = true;
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<CameraMovement>().enabled = true;
    }
    private void OnDisable()
    {

        GetComponent<SwarmInput>().enabled = false;
        GetComponent<SwarmUI>().enabled = false;
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<CameraMovement>().enabled = false;
    }
}
