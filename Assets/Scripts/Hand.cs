using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class Hand
{
    public bool IsHoldingObject => _currentHeldObject != null;
    
    INetworkGrabbable _currentHeldObject;
    Vector3 _lastFramePosition;
    Vector3 _direction;

    public void Grab(Vector3 grabPosition, float radius)
    {
        if (IsHoldingObject) return;
        Collider[] colliders = Physics.OverlapSphere(grabPosition, radius);
        Dictionary<INetworkGrabbable, Transform> grabbables = new();
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out INetworkGrabbable grabbable))
            {
                grabbables.Add(grabbable, collider.transform);
            }
        }
        if (grabbables.Count == 0) return;

        KeyValuePair<INetworkGrabbable, Transform> closestGrabbable = new(null, null);
        float maxDistance = float.MaxValue;
        
        foreach (KeyValuePair<INetworkGrabbable, Transform> grabbable in grabbables)
        {
            float distance = Vector3.Distance(grabPosition, grabbable.Value.position);
            if (distance < maxDistance)
            {
                maxDistance = distance;
                closestGrabbable = grabbable;
            }
        }
        _currentHeldObject = closestGrabbable.Key;
        
        _currentHeldObject.Grabbed();
        DebugConsole.Log("Grabbed Object");
    }

    public void Grabbing(Vector3 position)
    {
        if (position.y <= WaterDeathController.WaterHeight)
        {
            _direction = Vector3.zero;
            Release();
            return;
        }
        
        _direction = VectorUtils.Direction(_lastFramePosition, position);
        
        _currentHeldObject.Grabbing(position);
        
        _lastFramePosition = position;
    }

    public void Release()
    {
        if (!IsHoldingObject) return;
        _currentHeldObject.Released(_direction);
        _currentHeldObject = null;
    }
}
