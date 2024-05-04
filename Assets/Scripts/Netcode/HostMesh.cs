using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] ClientMesh clientMeshPrefab;
    Transform sceneParent;
    
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
        // Mesh mesh = meshFilter.mesh;
        sceneParent = FindObjectOfType<RoomEnvironmentInitializer>().transform;

        if (sceneParent == null)
        {
            DebugConsole.Error("RoomEnvironmentInitializer not found");
            return;
        }


        if (sceneParent.childCount <= 0 || sceneParent.GetChild(0).childCount <= 0)
        {
            DebugConsole.Error("Scene Room not found");
            return;
        }
        
        OVRSceneRoom room = sceneParent.GetChild(0).GetComponent<OVRSceneRoom>();
        
        if (room != null)
        {
            DebugConsole.Success("Found Scene Room");
        }

        Mesh mesh = sceneParent.GetChild(3).GetComponent<MeshFilter>().mesh;
        
        ClientMesh clientMesh = Instantiate(clientMeshPrefab);

        foreach (var v in mesh.vertices) clientMesh.vertices.Add(v);
        foreach (var n in mesh.normals) clientMesh.normals.Add(n);
        foreach (var t in mesh.triangles) clientMesh.triangles.Add(t);
        foreach (var u in mesh.uv) clientMesh.uvs.Add(u);

        clientMesh.NetworkObject.Spawn(true);
    }
}