using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeshDestroyer : MonoBehaviour
{
    [SerializeField] SerializedWaitForSeconds findRoomInterval;

    [SerializeField] float destroyRadius = 0.25f;
    
    MeshFilter _sceneMesh;

    IEnumerator Start()
    {
        findRoomInterval.Init();
        var room = FindObjectOfType<OVRSceneRoom>();
        while (room == null)
        {
            room = FindObjectOfType<OVRSceneRoom>();
            yield return findRoomInterval.Wait;
        }
        
        while (room.transform.childCount <= 0 || _sceneMesh == null)
        {
            yield return findRoomInterval.Wait;
        }
        
        
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
            
            Destroy(child.gameObject);
        }
    }

    public void DestroyWalls(MeshFilter meshFilter)
    {
        _sceneMesh = meshFilter;
        /*Mesh mesh = meshFilter.mesh;

        List<int> triangles = mesh.triangles.ToList();
        Vector3 position = new Vector3(0, transform.position.y, distance);
        float angle = 0;
        float increment = 360f / resolution;
        
        for (int i = 0; i < resolution; i++)
        {
            angle += increment;
            position = RotatePointAroundPivot(position, Vector3.up, angle);

            int hitTriangle = GetHitTriangle(position, meshFilter);
            if (hitTriangle != -1 && hitTriangle * 3 + 2 < triangles.Count)
            {
                triangles.RemoveAt(hitTriangle * 3);
                triangles.RemoveAt(hitTriangle * 3);
                triangles.RemoveAt(hitTriangle * 3);
            }
            else
            {
                Debug.LogWarning("Invalid hit triangle index: " + hitTriangle);
            }
        }
        
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();*/
    }

    /*int GetHitTriangle(Vector3 shootPoint, MeshFilter meshFilter)
    {
        if (!Physics.Raycast(shootPoint, VectorUtils.Direction(shootPoint, new Vector3(0, transform.position.y, 0)), out RaycastHit hit, 1000, meshLayer))
        {
            Debug.LogError("No Hit Detected");
            return -1;
        }
     
        Mesh mesh = meshFilter.mesh;
        Vector3 localHitPoint = meshFilter.transform.InverseTransformPoint(hit.point);
        int closestTriangleIndex = GetClosestTriangleIndex(localHitPoint, mesh);
        return closestTriangleIndex;
    }*/

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) 
    {
        var dir  = point - pivot;
        dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        point = dir + pivot;
        return point;
    }
    
    static bool PointInBox (Vector3 point, BoxCollider box )
    {
        point = box.transform.InverseTransformPoint( point ) - box.center;
		
        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);

        return point.x < halfX && point.x > -halfX && 
               point.y < halfY && point.y > -halfY && 
               point.z < halfZ && point.z > -halfZ;
    }

    static void DestroyInBox(MeshFilter meshFilter, BoxCollider box)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        List<int> triangles = mesh.triangles.ToList();

        // Iterate through triangles in reverse order to avoid index shifting
        for (int i = triangles.Count - 3; i >= 0; i -= 3)
        {
            Vector3 v0 = meshFilter.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = meshFilter.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = meshFilter.transform.TransformPoint(vertices[triangles[i + 2]]);
            
            if (PointInBox(v0, box) || PointInBox(v1, box) || PointInBox(v2, box))
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
    
    static int GetClosestTriangleIndex(Vector3 point, Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh not assigned.");
            return -1;
        }

        int closestTriangleIndex = -1;
        float closestDistance = float.MaxValue;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector3 closestPointOnTriangle = ClosestPointOnTriangle(point, v0, v1, v2);
            float distance = Vector3.Distance(point, closestPointOnTriangle);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTriangleIndex = i / 3;
            }
        }

        return closestTriangleIndex;
    }

    static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = p - a;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0f && d2 <= 0f)
            return a;

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0f && d4 <= d3)
            return b;

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);
            return a + v * ab;
        }

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0f && d5 <= d6)
            return c;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w = d2 / (d2 - d6);
            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;
        if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return b + w * (c - b);
        }

        float denom = 1f / (va + vb + vc);
        float vs = vb * denom;
        float ws = vc * denom;
        return a + ab * vs + ac * ws;
    }

    /*void OnDrawGizmos()
    {
        Vector3 position = new Vector3(0, transform.position.y, distance);
        float angle = 0;
        float increment = 360f / resolution;
        
        for (int i = 0; i < resolution; i++)
        {
            angle += increment;
            position = RotatePointAroundPivot(position, Vector3.up, angle);
            
            Gizmos.DrawLine(position, new Vector3(0, transform.position.y, 0));
        }
    }*/
}
