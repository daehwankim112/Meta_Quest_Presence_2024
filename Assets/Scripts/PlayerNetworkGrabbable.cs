using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class PlayerNetworkGrabbable : NetworkBehaviour, INetworkGrabbable
{
    public void Grabbed()
    {
        if (IsServer) return;
        
        var idList = new List<ulong>() { OwnerClientId };
        TestClientRpc(new ClientRpcParams() { Send = {TargetClientIds = idList} });
    }

    public void Released()
    {

    }

    [ClientRpc]
    void TestClientRpc(ClientRpcParams rpcParams)
    {
        if (!rpcParams.Send.TargetClientIds.Contains(OwnerClientId)) return;
        
        DebugConsole.Success($"Recieved grab? {OwnerClientId}");
    }
}
