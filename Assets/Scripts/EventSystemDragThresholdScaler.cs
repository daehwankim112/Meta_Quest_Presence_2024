using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDragThresholdScaler : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    int _initialDragThreshold;
    
    void Start()
    {
        _initialDragThreshold = eventSystem.pixelDragThreshold;
        eventSystem.pixelDragThreshold = _initialDragThreshold * Mathf.RoundToInt(RoomEnvironmentInitializer.RoomScale.magnitude);
        DebugConsole.Log("Drag Threshold: " + RoomEnvironmentInitializer.RoomScale.magnitude);
    }

    void OnEnable()
    {
        GameEvents.OnLobbyHosted += ScaleUp;
        GameEvents.OnLobbyJoined += ScaleDown;
    }

    void OnDisable()
    {
        GameEvents.OnLobbyHosted -= ScaleUp;
        GameEvents.OnLobbyJoined -= ScaleDown;
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
