
using UnityEngine;
using UnityEngine.UIElements;

public class SwarmPlayer : Player
{

    public int EntityLimit = 6;
    public SwarmUI UIref;
    private void Start()
    {
        UIref = GetComponent<SwarmUI>();
        if(UIref.enabled)
        {
            GetComponent<UIDocument>().enabled = true;
        }
    }
    private void Update()
    {
        if (BaseLocation != null && GetComponentInParent<SwitchPlayer>().currentPlayer == 1)
        {
            GetComponentInParent<SwarmPlayerNet>().AskSpawnEntityServerRpc(1, BaseLocation._gridPositionX + Random.Range(0,3), BaseLocation._gridPositionZ + Random.Range(0, 3));
        }
    }
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
