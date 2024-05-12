using NuiN.NExtensions;
using UnityEngine;

public class SmallPlayerGrabbedController : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] disableOnGrab;
    
    void OnEnable()
    {
        GameEvents.OnLocalClientGrabbed += DisableScripts;
        GameEvents.OnLocalClientReleased += EnableScripts;
        GameEvents.OnLocalClientBeingGrabbed += MoveToHandPosition;
    }

    void OnDisable()
    {
        GameEvents.OnLocalClientGrabbed -= DisableScripts;
        GameEvents.OnLocalClientReleased -= EnableScripts;
        GameEvents.OnLocalClientBeingGrabbed -= MoveToHandPosition;
    }

    void MoveToHandPosition(Vector3 position)
    {
        transform.position = position;
    }

    void DisableScripts()
    {
        disableOnGrab.ForEach(item => item.enabled = false);
    }

    void EnableScripts()
    {
        disableOnGrab.ForEach(item => item.enabled = true);
    }
}
