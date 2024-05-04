using Unity.Netcode;
using UnityEngine;

public readonly struct NetworkMesh
{
    public readonly NetworkList<Vector3> vertices;
    public readonly NetworkList<Vector3> normals;
    public readonly NetworkList<int> triangles;

    public NetworkMesh(Mesh mesh)
    {
        vertices = new NetworkList<Vector3>();
        normals = new NetworkList<Vector3>();
        triangles = new NetworkList<int>();
        
        foreach (var v in mesh.vertices) vertices.Add(v);
        foreach (var n in mesh.normals) normals.Add(n);
        foreach (var t in mesh.triangles) triangles.Add(t);
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = new Vector3[vertices.Count],
            normals = new Vector3[normals.Count],
            triangles = new int[triangles.Count]
        };

        for (int i = 0; i < vertices.Count; i++) mesh.vertices[i] = vertices[i];
        for (int i = 0; i < normals.Count; i++) mesh.normals[i] = normals[i];
        for (int i = 0; i < triangles.Count; i++) mesh.triangles[i] = triangles[i];
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }
}