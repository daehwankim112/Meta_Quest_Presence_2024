using NuiN.NExtensions;
using UnityEngine;

public class SmallPlayerGrabbedController : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] disableOnGrab;
    [SerializeField] Rigidbody rb;
    
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
        
        rb.isKinematic = true;
    }

    void EnableScripts()
    {
        disableOnGrab.ForEach(item => item.enabled = true);
        
        rb.isKinematic = false;
    }
}
