using Unity.Netcode;
using UnityEngine;

public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    
    NetworkList<Vector3> _vertices;
    NetworkList<Vector3> _normals;
    NetworkList<Vector2> _uvs;
    NetworkList<int> _triangles;

    void Awake()
    {
        _vertices = new NetworkList<Vector3>();
        _normals = new NetworkList<Vector3>();
        _triangles = new NetworkList<int>();
        _uvs = new NetworkList<Vector2>();
        
        _vertices.Initialize(this);
        _normals.Initialize(this);
        _triangles.Initialize(this);
        _uvs.Initialize(this);
    }

    public void Initialize(Mesh mesh)
    {
        foreach (var v in mesh.vertices) _vertices.Add(v);
        foreach (var n in mesh.normals) _normals.Add(n);
        foreach (var t in mesh.triangles) _triangles.Add(t);
        foreach (var u in mesh.uv) _uvs.Add(u);
    }

    public override void OnNetworkSpawn()
    {
        var vL = new Vector3[_vertices.Count];
        var nL = new Vector3[_normals.Count];
        var tL = new int[_triangles.Count];
        var uL = new Vector2[_uvs.Count];

        for (int i = 0; i < _vertices.Count; i++) vL[i] = _vertices[i];
        for (int i = 0; i < _normals.Count; i++) nL[i] = _normals[i];
        for (int i = 0; i < _triangles.Count; i++) tL[i] = _triangles[i];
        for (int i = 0; i < _uvs.Count; i++) uL[i] = _uvs[i];

        Mesh mesh = new();
        mesh.SetVertices(vL);
        mesh.SetNormals(nL);
        mesh.triangles = tL;
        mesh.uv = uL;
        
        meshFilter.mesh = mesh;
    }
}
