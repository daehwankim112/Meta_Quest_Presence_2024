using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static void InvokeSceneMeshInitalized(MeshFilter meshFilter, List<Vector3> treePositions, List<Quaternion> treeRotations) => OnSceneMeshInitialized?.Invoke(meshFilter, treePositions, treeRotations);
    public static event Action<MeshFilter, List<Vector3>, List<Quaternion>> OnSceneMeshInitialized;

    public static void InvokeLobbyHosted() => OnLobbyHosted?.Invoke();
    public static event Action OnLobbyHosted;
    
    public static void InvokeLobbyJoined() => OnLobbyJoined?.Invoke();
    public static event Action OnLobbyJoined;

    public static void InvokeRecievedSceneMeshFromHost(Vector3 spawnPosition) => OnRecievedSceneMeshFromHost?.Invoke(spawnPosition);
    public static event Action<Vector3> OnRecievedSceneMeshFromHost;

    public static void InvokeRoomScaled() => OnRoomScaled?.Invoke();
    public static event Action OnRoomScaled;

    public static void InvokeLocalClientGrabbed() => OnLocalClientGrabbed?.Invoke();
    public static event Action OnLocalClientGrabbed;
    
    public static void InvokeLocalClientReleased(Vector3 direction) => OnLocalClientReleased?.Invoke(direction);
    public static event Action<Vector3> OnLocalClientReleased;

    public static void InvokeLocalClientBeingGrabbed(Vector3 position) => OnLocalClientBeingGrabbed?.Invoke(position);
    public static event Action<Vector3> OnLocalClientBeingGrabbed;

    public static void InvokePlayerFellInWater(SmallPlayer smallPlayer) => OnSmallPlayerFellInWater?.Invoke(smallPlayer);
    public static event Action<SmallPlayer> OnSmallPlayerFellInWater;
}
