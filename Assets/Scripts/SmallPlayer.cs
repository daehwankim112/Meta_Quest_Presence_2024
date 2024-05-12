using NuiN.Movement;
using NuiN.NExtensions;
using UnityEngine;

public class SmallPlayer : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GroundMovementController movement;

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
            GameEvents.InvokePlayerFellInWater(this);
            rb.velocity = rb.velocity.With(z:0, x: 0);
            rb.angularVelocity = Vector3.zero;
        }
    }
}
