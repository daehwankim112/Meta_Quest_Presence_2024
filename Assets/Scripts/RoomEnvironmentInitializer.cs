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

    void OnEnable()
    {
        GameEvents.OnLobbyJoined += DisableThis;
        GameEvents.OnLobbyHosted += DisableThis;
    }
    void OnDisable()
    {
        GameEvents.OnLobbyJoined -= DisableThis;
        GameEvents.OnLobbyHosted -= DisableThis;
    }

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
        var populatedTransforms = PopulateTrees();
        
        GameEvents.SceneMeshInitalized(_sceneMeshFilter, populatedTransforms.treePositions, populatedTransforms.treeRotations);
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
                
                RoomEnvironmentHelpers.RemoveTrianglesInBox(_sceneMeshFilter, boxCol);
            }
        }
        
        _sceneMeshFilter.mesh.RecalculateNormals();
        _sceneMeshFilter.mesh.RecalculateBounds();
        _sceneMeshFilter.mesh.Optimize();
        
        Destroy(_sceneMeshFilter.GetComponent<MeshCollider>());
        _sceneMeshFilter.gameObject.AddComponent<MeshCollider>();
        
        Destroy(room.gameObject);
    }

    (List<Vector3> treePositions, List<Quaternion> treeRotations) PopulateTrees()
    {
        Mesh mesh = _sceneMeshFilter.mesh;

        Vector2 randomNoisePosition = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));

        var validTreePositions = new List<Vector3>();
        var validTreeRotations = new List<Quaternion>();

        for (int i = 0; i < mesh.vertexCount; i += treeScarcity)
        {
            Vector3 vertice = _sceneMeshFilter.transform.TransformPoint(mesh.vertices[i]);
            float normalY = Mathf.Abs(mesh.normals[i].y);
            float noiseSample = Mathf.PerlinNoise(vertice.x + randomNoisePosition.x, vertice.z + randomNoisePosition.y);
            if (noiseSample >= treeNoiseThreshold && normalY < treeNormalThreshold && vertice.y > minHeight && treePrefab.ValidPlacement(vertice))
            {
                validTreePositions.Add(vertice);
                validTreeRotations.Add(Quaternion.Euler(0, Random.Range(0f, 360f), 0f));
            }
        }

        for (int i = 0; i < validTreePositions.Count; i++)
        {
            Instantiate(treePrefab, validTreePositions[i], validTreeRotations[i], _sceneMeshFilter.transform);
        }

        return (validTreePositions, validTreeRotations);
    }

    void DisableThis()
    {
        gameObject.SetActive(false);
    }
}