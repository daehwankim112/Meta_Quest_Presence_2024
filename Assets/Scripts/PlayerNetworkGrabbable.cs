using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkGrabbable : NetworkBehaviour, INetworkGrabbable
{
    NetworkVariable<Vector3> _grabbingPosition = new();
    NetworkVariable<bool> _isGrabbed = new();
    NetworkVariable<Vector3> _releaseDirection = new();

    public override void OnNetworkSpawn()
    {
        _grabbingPosition.Initialize(this);
        _isGrabbed.Initialize(this);
        _releaseDirection.Initialize(this);
        
        _grabbingPosition.OnValueChanged += BeingGrabbed;
        _isGrabbed.OnValueChanged += GrabbedOrReleased;
    }

    public override void OnNetworkDespawn()
    {
        _grabbingPosition.OnValueChanged -= BeingGrabbed;
        _isGrabbed.OnValueChanged -= GrabbedOrReleased;
    }

    void INetworkGrabbable.Grabbed() => _isGrabbed.Value = true;
    void INetworkGrabbable.Released(Vector3 direction)
    {
        _isGrabbed.Value = false;
        _releaseDirection.Value = direction;
    }

    void INetworkGrabbable.Grabbing(Vector3 position) => _grabbingPosition.Value = position;

    void BeingGrabbed(Vector3 oldVal, Vector3 newVal)
    {
        if (!IsOwner) return;
        
        GameEvents.InvokeLocalClientBeingGrabbed(newVal);
    }

    void GrabbedOrReleased(bool oldVal, bool newVal)
    {
        if (!IsOwner) return;

        if (newVal)
        {
            GameEvents.InvokeLocalClientGrabbed();
            DebugConsole.Success("Was Grabbed!");
        }
        else
        {
            GameEvents.InvokeLocalClientReleased(_releaseDirection.Value);
            DebugConsole.Log("Was Released!");
        }
    }
}
