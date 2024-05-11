using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkGrabbable : NetworkBehaviour, INetworkGrabbable
{
    void INetworkGrabbable.Grabbed()
    {
        var idList = new List<ulong>() { OwnerClientId };
        GrabbedClientRpc(new ClientRpcParams() { Send = {TargetClientIds = idList} });
    }
    
    void INetworkGrabbable.Released()
    {
        var idList = new List<ulong>() { OwnerClientId };
        ReleasedClientRpc(new ClientRpcParams() { Send = {TargetClientIds = idList} });
    }

    void INetworkGrabbable.Grabbing(Vector3 position)
    {
        var idList = new List<ulong>() { OwnerClientId };
        BeingGrabbedClientRpc(position, new ClientRpcParams() { Send = { TargetClientIds = idList } });
    }

    [ClientRpc]
    void GrabbedClientRpc(ClientRpcParams rpcParams)
    {
        if (!rpcParams.Send.TargetClientIds.Contains(OwnerClientId)) return;
        
        DebugConsole.Success("Was Grabbed!");
        GameEvents.InvokeLocalClientGrabbed();
    }
    
    [ClientRpc]
    void BeingGrabbedClientRpc(Vector3 position, ClientRpcParams rpcParams)
    {
        if (!rpcParams.Send.TargetClientIds.Contains(OwnerClientId)) return;
        
        DebugConsole.Log($"Being grabbed at: {position}");
        GameEvents.InvokeLocalClientBeingGrabbed(position);
    }
    
    [ClientRpc]
    void ReleasedClientRpc(ClientRpcParams rpcParams)
    {
        if (!rpcParams.Send.TargetClientIds.Contains(OwnerClientId)) return;
        
        DebugConsole.Log("Was Released!");
        GameEvents.InvokeLocalClientReleased();
    }
}
