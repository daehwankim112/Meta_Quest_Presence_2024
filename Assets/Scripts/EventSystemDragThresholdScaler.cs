using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDragThresholdScaler : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    int _initialDragThreshold;
    
    void Awake()
    {
        _initialDragThreshold = eventSystem.pixelDragThreshold;
        eventSystem.pixelDragThreshold = _initialDragThreshold * Mathf.RoundToInt(RoomEnvironmentInitializer.RoomScale.magnitude);
    }

    void OnEnable()
    {
        GameEvents.OnLobbyHosted += ScaleUp;
        GameEvents.OnRoomScaled += ScaleDown;
    }

    void OnDisable()
    {
        GameEvents.OnLobbyHosted -= ScaleUp;
        GameEvents.OnRoomScaled -= ScaleDown;
    }
    
    void ScaleDown()
    {
        eventSystem.pixelDragThreshold = _initialDragThreshold;
    }

    void ScaleUp()
    {
        eventSystem.pixelDragThreshold = _initialDragThreshold * Mathf.RoundToInt(RoomEnvironmentInitializer.RoomScale.magnitude);
    }
}
