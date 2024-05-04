using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class HostMesh : NetworkBehaviour
{
    MeshFilter _meshFilter;
    [SerializeField] ClientMesh clientMeshPrefab;

    [SerializeField] InputAction spawnMeshAction;

    void OnEnable()
    {
        spawnMeshAction.Enable();
        spawnMeshAction.performed += HandleInputAction;
        GameEvents.OnSceneMeshInitialized += SetMeshFilter;
    }
    void OnDisable()
    {
        spawnMeshAction.performed -= HandleInputAction;
        GameEvents.OnSceneMeshInitialized -= SetMeshFilter;
    }

    void SetMeshFilter(MeshFilter filter)
    {
        _meshFilter = filter;
    }

    void HandleInputAction(InputAction.CallbackContext ctx)
    {
        CreateMeshObject();
    }

    void CreateMeshObject()
    {
        if (!IsHost || !IsOwner) return;
        
        if (_meshFilter == null)
        {
            DebugConsole.Error("Room Environment not initialized when trying to send to clients");
            return;
        }
        
        ClientMesh clientMesh = Instantiate(clientMeshPrefab);

        Mesh mesh = _meshFilter.mesh;
        
        clientMesh.Initialize(mesh);

        clientMesh.transform.position = _meshFilter.transform.position;
        clientMesh.transform.rotation = _meshFilter.transform.rotation;
        clientMesh.transform.localScale = _meshFilter.transform.localScale;

        clientMesh.NetworkObject.Spawn(true);
    }
}