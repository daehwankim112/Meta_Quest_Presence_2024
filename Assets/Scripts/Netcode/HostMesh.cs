using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    NetworkList<Vector3> _vertices;
    NetworkList<Vector3> _normals;
    NetworkList<int> _triangles;

    MeshFilter _meshFilter;

    [SerializeField] ClientMesh clientMeshPrefab;

    void Awake()
    {
        _vertices = new NetworkList<Vector3>();
        _normals = new NetworkList<Vector3>();
        _triangles = new NetworkList<int>();
        
        _vertices.Initialize(this);
        _normals.Initialize(this);
        _triangles.Initialize(this);
        
        DebugConsole.Log("SWAAAAG");
        
        GameEvents.OnSceneMeshInitialized += SetMeshNetworkVariables;
    }

    void OnDisable()
    {
        GameEvents.OnSceneMeshInitialized -= SetMeshNetworkVariables;
    }

    void SetMeshNetworkVariables(MeshFilter meshFilter)
    {
        _meshFilter = meshFilter;
    }

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
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CreateMeshObject();
        }
    }

    void CreateMeshObject()
    {
        ClientMesh clientMesh = Instantiate(clientMeshPrefab);
        //clientMesh.SetMesh(CreateMesh());
        clientMesh.NetworkObject.Spawn(true);
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
        
        /*mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();*/

        return mesh;
    }
}