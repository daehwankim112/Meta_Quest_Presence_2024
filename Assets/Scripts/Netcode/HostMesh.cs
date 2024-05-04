using System;
using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    MeshFilter _meshFilter;
    [SerializeField] ClientMesh clientMeshPrefab;
    
    void Update()
    {
        if (!IsHost || !IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateMeshObject();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSceneMeshInitialized += SetMeshFilter;
    }
    private void OnDisable()
    {
        GameEvents.OnSceneMeshInitialized -= SetMeshFilter;
    }

    private void SetMeshFilter(MeshFilter filter)
    {
        _meshFilter = filter;
    }

    void CreateMeshObject()
    {
        if (_meshFilter == null)
        {
            DebugConsole.Error("Room Environment not initialized when trying to send to clients");
            return;
        }
        
        ClientMesh clientMesh = Instantiate(clientMeshPrefab);

        Mesh mesh = _meshFilter.mesh;
        
        foreach (var v in mesh.vertices) clientMesh.vertices.Add(v);
        foreach (var n in mesh.normals) clientMesh.normals.Add(n);
        foreach (var t in mesh.triangles) clientMesh.triangles.Add(t);
        foreach (var u in mesh.uv) clientMesh.uvs.Add(u);

        clientMesh.transform.position = _meshFilter.transform.position;
        clientMesh.transform.rotation = _meshFilter.transform.rotation;
        clientMesh.transform.localScale = _meshFilter.transform.localScale;

        clientMesh.NetworkObject.Spawn(true);
    }
}