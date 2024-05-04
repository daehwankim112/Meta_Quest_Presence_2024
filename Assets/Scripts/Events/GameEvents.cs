using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action<MeshFilter, List<Vector3>, List<Quaternion>> OnSceneMeshInitialized;
    public static void SceneMeshInitalized(MeshFilter meshFilter, List<Vector3> treePositions, List<Quaternion> treeRotations) => OnSceneMeshInitialized?.Invoke(meshFilter, treePositions, treeRotations);
}
