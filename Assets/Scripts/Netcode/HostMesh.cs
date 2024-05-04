using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] ClientMesh clientMeshPrefab;
    
    void Update()
    {
        if (!IsHost || !IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateMeshObject();
        }
    }

    void CreateMeshObject()
    {
        Mesh mesh = meshFilter.mesh;

        ClientMesh clientMesh = Instantiate(clientMeshPrefab);

        foreach (var v in mesh.vertices) clientMesh.vertices.Add(v);
        foreach (var n in mesh.normals) clientMesh.normals.Add(n);
        foreach (var t in mesh.triangles) clientMesh.triangles.Add(t);
        foreach (var u in mesh.uv) clientMesh.uvs.Add(u);

        clientMesh.NetworkObject.Spawn(true);
    }
}