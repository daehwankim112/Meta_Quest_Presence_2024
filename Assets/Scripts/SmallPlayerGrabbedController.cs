using NuiN.NExtensions;
using UnityEngine;

public class SmallPlayerGrabbedController : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] disableOnGrab;
    [SerializeField] Rigidbody rb;
    
    void OnEnable()
    {
        GameEvents.OnLocalClientGrabbed += Grabbed;
        GameEvents.OnLocalClientReleased += Released;
        GameEvents.OnLocalClientBeingGrabbed += MoveToHandPosition;
    }

    void OnDisable()
    {
        GameEvents.OnLocalClientGrabbed -= Grabbed;
        GameEvents.OnLocalClientReleased -= Released;
        GameEvents.OnLocalClientBeingGrabbed -= MoveToHandPosition;
    }

    void MoveToHandPosition(Vector3 position)
    {
        transform.position = position;
    }

    void Grabbed()
    {
        disableOnGrab.ForEach(item => item.enabled = false);
        
        rb.isKinematic = true;
    }

    void Released(Vector3 direction)
    {
        disableOnGrab.ForEach(item => item.enabled = true);
        
        rb.isKinematic = false;
        rb.AddForce(direction * 100, ForceMode.VelocityChange);
    }
}
