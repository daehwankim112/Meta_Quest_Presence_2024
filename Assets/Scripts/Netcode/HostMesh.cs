using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    NetworkMesh _networkMesh;
    
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
        _networkMesh = new NetworkMesh(meshFilter.mesh);
        SendMeshClientRPC();
    }

    [ClientRpc]
    void SendMeshClientRPC()
    {
        if (IsHost) return;
        
        GameObject sceneMesh = new GameObject("HostMesh");
        sceneMesh.AddComponent<MeshFilter>().mesh = _networkMesh.CreateMesh();
    }
}