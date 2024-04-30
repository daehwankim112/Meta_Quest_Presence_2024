using System.Collections.Generic;
using System.Linq;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeshDestroyer : MonoBehaviour
{
    [SerializeField] LayerMask meshLayer;

    [SerializeField] int resolution = 100;
    [SerializeField] float distance = 15f;

    public void DestroyWalls(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.mesh;

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
        mesh.Optimize();
    }

    int GetHitTriangle(Vector3 shootPoint, MeshFilter meshFilter)
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
    }
    
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) 
    {
        var dir  = point - pivot;
        dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        point = dir + pivot;
        return point;
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

    void OnDrawGizmos()
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
    }
}
