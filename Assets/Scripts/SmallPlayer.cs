using NuiN.Movement;
using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

public class SmallPlayer : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GroundMovementController movement;

    void OnEnable()
    {
        GameEvents.OnLocalClientGrabbed += Grabbed;
        GameEvents.OnLocalClientReleased += Released;
        GameEvents.OnLocalClientBeingGrabbed += MoveToHandPosition;
        GameEvents.OnSetPlayerPosition += SetPosition;
    }

    void OnDisable()
    {
        GameEvents.OnLocalClientGrabbed -= Grabbed;
        GameEvents.OnLocalClientReleased -= Released;
        GameEvents.OnLocalClientBeingGrabbed -= MoveToHandPosition;
        GameEvents.OnSetPlayerPosition -= SetPosition;
    }

    void MoveToHandPosition(Vector3 position)
    {
        transform.position = position;
    }

    void Grabbed()
    {
        movement.DisableMovement();
    }

    void Released(Vector3 direction)
    {
        movement.EnableMovement();
        rb.AddForce(direction * 100, ForceMode.VelocityChange);
    }

    void Update()
    {
        if (transform.position.y <= WaterDeathController.WaterHeight)
        {
            GameEvents.InvokePlayerFellInWater(NetworkObject.OwnerClientId);
            rb.velocity = rb.velocity.With(z:0, x: 0);
            rb.angularVelocity = Vector3.zero;
        }
    }

    void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
