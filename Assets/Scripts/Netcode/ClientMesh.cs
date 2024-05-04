using Unity.Netcode;
using UnityEngine;

public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    
    public NetworkList<Vector3> vertices;
    public NetworkList<Vector3> normals;
    public NetworkList<int> triangles;

    void Awake()
    {
        vertices = new NetworkList<Vector3>();
        normals = new NetworkList<Vector3>();
        triangles = new NetworkList<int>();
        
        vertices.Initialize(this);
        normals.Initialize(this);
        triangles.Initialize(this);
    }

    public override void OnNetworkSpawn()
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

        meshFilter.mesh = mesh;
    }
}
