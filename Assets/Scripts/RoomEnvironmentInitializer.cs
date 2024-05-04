using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

public class RoomEnvironmentInitializer : MonoBehaviour
{
    [SerializeField] SerializedWaitForSeconds findRoomInterval;
    [SerializeField] Transform sceneParent;

    [SerializeField] float destroyRadius = 0.25f;

    [SerializeField] Tree treePrefab;
    [SerializeField] int treeScarcity;
    [SerializeField] float minHeight = 0.3f;
    [SerializeField] float treeNoiseThreshold;
    [SerializeField] float treeNormalThreshold;

    float _roomScale;
    MeshFilter _sceneMeshFilter;

    IEnumerator Start()
    {
        const string spatialPermission = "com.oculus.permission.USE_SCENE";
        bool hasUserAuthorizedPermission = UnityEngine.Android.Permission.HasUserAuthorizedPermission(spatialPermission);

        if (!hasUserAuthorizedPermission) DebugConsole.Warn("Please enable the permission in the Oculus app");
        else DebugConsole.Success("com.oculus.permission.USE_SCENE Permission granted");

        findRoomInterval.Init();

        while (sceneParent.childCount <= 0 || sceneParent.GetChild(0).childCount <= 0)
        {
            yield return null;
        }
        OVRSceneRoom room = sceneParent.GetChild(0).GetComponent<OVRSceneRoom>();

        if (room != null)
        {
            DebugConsole.Success("Found Scene Room");
        }

        while (_sceneMeshFilter == null || _sceneMeshFilter.mesh.vertexCount <= 0)
        {
            yield return null;
        }

        DebugConsole.Success("Found Scene Mesh");
        yield return findRoomInterval.Wait;
        
        DestroyWalls(room);
        RefreshSceneMeshCollider();
        PopulateTrees();
        
        GameEvents.SceneMeshInitalized(_sceneMeshFilter);
    }

    public void CacheSceneMesh(MeshFilter meshFilter)
    {
        _sceneMeshFilter = meshFilter;
        DebugConsole.Success("Scene mesh cached");
        
        meshFilter.transform.SetParent(transform);
    }

    void DestroyWalls(OVRSceneRoom room)
    {
        foreach (Transform child in room.transform)
        {
            float childMag = child.position.magnitude;
            if (childMag > _roomScale) _roomScale = childMag;
            
            if (!child.TryGetComponent(out OVRSemanticClassification classification)) continue;
            IReadOnlyList<string> labels = classification.Labels;
            
            if (labels.Contains("WALL_FACE") || labels.Contains("DOOR_FRAME") || labels.Contains("CEILING"))
            {
                BoxCollider boxCol = child.gameObject.AddComponent<BoxCollider>();
                boxCol.size = boxCol.size.With(z: destroyRadius);
                
                RoomEnvironmentHelpers.DestroyInBox(_sceneMeshFilter, boxCol);
            }
        }

        Destroy(room.gameObject);
    }

    void RefreshSceneMeshCollider()
    {
        Destroy(_sceneMeshFilter.GetComponent<MeshCollider>());
        _sceneMeshFilter.gameObject.AddComponent<MeshCollider>();
    }

    void PopulateTrees()
    {
        Mesh mesh = _sceneMeshFilter.mesh;

        Vector2 randomNoisePosition = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));

        var validPositions = new List<Vector3>();

        for (int i = 0; i < mesh.vertexCount; i += treeScarcity)
        {
            Vector3 vertice = _sceneMeshFilter.transform.TransformPoint(mesh.vertices[i]);
            float normalY = Mathf.Abs(mesh.normals[i].y);
            float noiseSample = Mathf.PerlinNoise(vertice.x + randomNoisePosition.x, vertice.z + randomNoisePosition.y);
            if (noiseSample >= treeNoiseThreshold && normalY < treeNormalThreshold && vertice.y > minHeight && treePrefab.ValidPlacement(vertice))
            {
                validPositions.Add(vertice);
            }
        }

        foreach (var position in validPositions)
        {
            Instantiate(treePrefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0f), _sceneMeshFilter.transform);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_roomScale, _roomScale, _roomScale) * 2);
    }
}