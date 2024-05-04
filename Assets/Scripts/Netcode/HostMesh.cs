using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class HostMesh : NetworkBehaviour
{
    List<Vector3> _treePositions;
    List<Quaternion> _treeRotations;
    MeshFilter _meshFilter;
    [SerializeField] ClientMesh clientMeshPrefab;

    void OnEnable()
    {
        GameEvents.OnSceneMeshInitialized += HandleInitialize;
        GameEvents.OnLobbyHosted += CreateMeshObject;
    }
    void OnDisable()
    {
        GameEvents.OnSceneMeshInitialized -= HandleInitialize;
        GameEvents.OnLobbyHosted -= CreateMeshObject;
    }

    void HandleInitialize(MeshFilter filter, List<Vector3> treePositions, List<Quaternion> treeRotations)
    {
        _meshFilter = filter;
        _treePositions = treePositions;
        _treeRotations = treeRotations;
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

        clientMesh.Initialize(_meshFilter.mesh, _treePositions, _treeRotations);

        clientMesh.transform.position = _meshFilter.transform.position;
        clientMesh.transform.rotation = _meshFilter.transform.rotation;
        clientMesh.transform.localScale = _meshFilter.transform.localScale;

        clientMesh.NetworkObject.Spawn(true);

        if (IsHost)
        {
            Destroy(clientMesh.gameObject);
        }
    }
}