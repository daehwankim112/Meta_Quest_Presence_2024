using Oculus.Interaction.PoseDetection.Debug.Editor.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public Transform HeldTransform { get; private set; }
    public bool IsHoldingObject => _currentHeldObject != null;
    private IGrabbable _currentHeldObject;
    float _grabRadius;

    public Hand(float grabRadius)
    {
        _grabRadius = grabRadius;
    }

    public void Grab(Vector3 grabPosition)
    {
        if (IsHoldingObject) return;
        Collider[] colliders = Physics.OverlapSphere(grabPosition, _grabRadius);
        Dictionary<IGrabbable, Transform> grabbables = new();
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IGrabbable grabbable))
            {
                grabbables.Add(grabbable, collider.transform);
            }
        }
        if (grabbables.Count == 0) return;

        KeyValuePair<IGrabbable, Transform> closestGrabbable = new(null, null);
        float maxDistance = float.MaxValue;
        
        foreach (var grabbable in grabbables)
        {
            float distance = Vector3.Distance(grabPosition, grabbable.Value.position);
            if (distance < maxDistance)
            {
                maxDistance = distance;
                closestGrabbable = grabbable;
            }
        }
        _currentHeldObject = closestGrabbable.Key;
        HeldTransform = closestGrabbable.Value;
        _currentHeldObject.Grabbed();
    }

    public void Release()
    {
        if (!IsHoldingObject) return;
        _currentHeldObject.Released();
        _currentHeldObject = null;
        HeldTransform = null;
    }

}
