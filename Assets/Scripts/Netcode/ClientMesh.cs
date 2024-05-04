using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    
    public NetworkList<Vector3> vertices;
    public NetworkList<Vector3> normals;
    public NetworkList<Vector2> uvs;
    public NetworkList<int> triangles;
    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>();
    public NetworkVariable<Vector3> scale = new NetworkVariable<Vector3>();

    void Awake()
    {
        vertices = new NetworkList<Vector3>();
        normals = new NetworkList<Vector3>();
        triangles = new NetworkList<int>();
        uvs = new NetworkList<Vector2>();
        
        vertices.Initialize(this);
        normals.Initialize(this);
        triangles.Initialize(this);
        uvs.Initialize(this);
    }

    public override void OnNetworkSpawn()
    {
        var vL = new List<Vector3>();
        var nL = new List<Vector3>();
        var tL = new List<int>();
        var uL = new List<Vector2>();

        foreach (var v in vertices) vL.Add(v);
        foreach (var n in normals) nL.Add(n);
        foreach (var t in triangles) tL.Add(t);
        foreach (var u in uvs) uL.Add(u);

        Mesh mesh = new()
        {
            vertices = vL.ToArray(),
            normals = nL.ToArray(),
            triangles = tL.ToArray(),
            uv = uL.ToArray()
        };
        
        meshFilter.mesh = mesh;
    }
}
