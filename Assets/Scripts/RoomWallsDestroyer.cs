using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.NExtensions;
using UnityEngine;

public class RoomWallsDestroyer : MonoBehaviour
{
    [SerializeField] SerializedWaitForSeconds findRoomInterval;

    [SerializeField] float destroyRadius = 0.25f;

    [SerializeField] TMPro.TMP_Text debugText;
    [SerializeField] OVRSceneManager sceneManager;
    
    MeshFilter _sceneMesh;

    IEnumerator Start()
    {
        const string spatialPermission = "com.oculus.permission.USE_SCENE";
        bool hasUserAuthorizedPermission = UnityEngine.Android.Permission.HasUserAuthorizedPermission(spatialPermission);

        if (!hasUserAuthorizedPermission) debugText.text += "Please enable the permission in the Oculus app";
        else debugText.text += "Permission granted";

        findRoomInterval.Init();
        var room = FindObjectOfType<OVRSceneRoom>();
        while (room == null)
        {
            room = FindObjectOfType<OVRSceneRoom>();
            yield return findRoomInterval.Wait;
        }

        yield return findRoomInterval.Wait;


        while (room.transform.childCount <= 0 || _sceneMesh == null)
        {
            yield return findRoomInterval.Wait;
        }

        yield return findRoomInterval.Wait;

        foreach (Transform child in room.transform)
        {
            if (!child.TryGetComponent(out OVRSemanticClassification classification)) continue;
            IReadOnlyList<string> labels = classification.Labels;
            
            if (labels.Contains("WALL_FACE") || labels.Contains("DOOR_FRAME") || labels.Contains("CEILING"))
            {
                BoxCollider boxCol = child.gameObject.AddComponent<BoxCollider>();
                boxCol.size = boxCol.size.With(z: destroyRadius);
                
                DestroyInBox(_sceneMesh, boxCol);
            }

        }

        Destroy(room.gameObject);
        Destroy(_sceneMesh.GetComponent<MeshCollider>());
        _sceneMesh.gameObject.AddComponent<MeshCollider>();

        Destroy(gameObject);
    }

    public void CacheSceneMesh(MeshFilter meshFilter)
    {
        _sceneMesh = meshFilter;
        debugText.text += "Scene mesh cached";
    }
    
    static bool BoxContainsPoint (Vector3 point, BoxCollider box )
    {
        point = box.transform.InverseTransformPoint( point ) - box.center;
		
        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);

        return point.x < halfX && point.x > -halfX && 
               point.y < halfY && point.y > -halfY && 
               point.z < halfZ && point.z > -halfZ;
    }

    void DestroyInBox(MeshFilter meshFilter, BoxCollider box)
    {
        debugText.text += "Destroying in box" + box.name;
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        List<int> triangles = mesh.triangles.ToList();

        for (int i = triangles.Count - 3; i >= 0; i -= 3)
        {
            Vector3 v0 = meshFilter.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = meshFilter.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = meshFilter.transform.TransformPoint(vertices[triangles[i + 2]]);
            
            if (BoxContainsPoint(v0, box) || BoxContainsPoint(v1, box) || BoxContainsPoint(v2, box))
            {
                triangles.RemoveAt(i + 2);
                triangles.RemoveAt(i + 1);
                triangles.RemoveAt(i);
            }
        }

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}