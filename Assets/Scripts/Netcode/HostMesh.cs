using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    NetworkList<Vector3> _vertices;
    NetworkList<Vector3> _normals;
    NetworkList<int> _triangles;
    NetworkList<Vector2> _uvs;

    [SerializeField] MeshFilter _meshFilter;

    [SerializeField] ClientMesh clientMeshPrefab;

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
        
        //GameEvents.OnSceneMeshInitialized += SetMeshNetworkVariables;
    }

    /*void OnDisable()
    {
        GameEvents.OnSceneMeshInitialized -= SetMeshNetworkVariables;
    }

    void SetMeshNetworkVariables(MeshFilter meshFilter)
    {
        _meshFilter = meshFilter;
    }*/

    void Update()
    {
        Debug.LogError(_vertices.Count);
        if (!IsHost || !IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Mesh mesh = _meshFilter.mesh;
        
            foreach (var v in mesh.vertices) _vertices.Add(v);
            foreach (var n in mesh.normals) _normals.Add(n);
            foreach (var t in mesh.triangles) _triangles.Add(t);
            foreach (var u in mesh.uv) _uvs.Add(u);
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CreateMeshObject();
        }
    }

    void CreateMeshObject()
    {
        ClientMesh clientMesh = Instantiate(clientMeshPrefab);

        foreach (var v in _vertices) clientMesh.vertices.Add(v);
        foreach (var n in _normals) clientMesh.normals.Add(n);
        foreach (var t in _triangles) clientMesh.triangles.Add(t);
        foreach (var u in _uvs) clientMesh.uvs.Add(u);

        clientMesh.NetworkObject.Spawn(true);
    }
}