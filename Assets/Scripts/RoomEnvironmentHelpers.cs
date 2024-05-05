using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RoomEnvironmentHelpers
{
    public static bool BoxContainsPoint (Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint( point ) - box.center;
		
        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);

        return point.x < halfX && point.x > -halfX && 
               point.y < halfY && point.y > -halfY && 
               point.z < halfZ && point.z > -halfZ;
    }

    public static void RemoveTrianglesInBox(MeshFilter meshFilter, BoxCollider box)
    {
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
    }
    
    public static Vector3 PivotVector3(Vector3 point, Vector3 pivot, Quaternion rotation) 
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(rotation.eulerAngles) * dir;
        point = dir + pivot;
        return point;
    }
}