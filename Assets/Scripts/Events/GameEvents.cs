using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<MeshFilter> OnSceneMeshInitialized;
    public static void SceneMeshInitalized(MeshFilter meshFilter) => OnSceneMeshInitialized?.Invoke(meshFilter);
}
