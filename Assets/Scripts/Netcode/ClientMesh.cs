using Unity.Netcode;
using UnityEngine;

public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;

    public void SetMesh(Mesh mesh)
    {
        DebugConsole.Success("Hello");
        meshFilter.mesh = mesh;
    }
}
