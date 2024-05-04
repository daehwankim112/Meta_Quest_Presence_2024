using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    NetworkList<Vector3> _vertices;
    NetworkList<Vector3> _normals;
    NetworkList<int> _triangles;

    MeshFilter _meshFilter;
    
    void OnEnable()
    {
        if (IsHost)
        {
            GameEvents.OnSceneMeshInitialized += SetMesh;
        }
    }
    void OnDisable()
    {
        if (IsHost)
        {
            GameEvents.OnSceneMeshInitialized -= SetMesh;
        }
    }

    void SetMesh(MeshFilter meshFilter)
    {
        _meshFilter = meshFilter;
    }

    void Update()
    {
        if (!IsOwner || _meshFilter == null) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetMeshValues(_meshFilter.mesh);
            CreateMeshOnClientClientRpc();
        }
    }

    [ClientRpc]
    void CreateMeshOnClientClientRpc()
    {
        GameObject sceneMesh = new GameObject("HostMesh");
        sceneMesh.AddComponent<MeshFilter>().mesh = CreateMesh();
    }

    void SetMeshValues(Mesh mesh)
    {
        _vertices = new NetworkList<Vector3>();
        _normals = new NetworkList<Vector3>();
        _triangles = new NetworkList<int>();
        
        foreach (var v in mesh.vertices) _vertices.Add(v);
        foreach (var n in mesh.normals) _normals.Add(n);
        foreach (var t in mesh.triangles) _triangles.Add(t);
    }
    
    Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = new Vector3[_vertices.Count],
            normals = new Vector3[_normals.Count],
            triangles = new int[_triangles.Count]
        };

        for (int i = 0; i < _vertices.Count; i++) mesh.vertices[i] = _vertices[i];
        for (int i = 0; i < _normals.Count; i++) mesh.normals[i] = _normals[i];
        for (int i = 0; i < _triangles.Count; i++) mesh.triangles[i] = _triangles[i];
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }
}