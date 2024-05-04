using Unity.Netcode;
using UnityEngine;

public class HostMesh : NetworkBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SendMeshClientRPC();
        }
    }

    [ClientRpc]
    void SendMeshClientRPC()
    {
        DebugConsole.Log("hello client");
    }
}