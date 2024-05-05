// David Kim 2024/4/26
// Description: Use Netcode to synchronize the player's head, hands, and root. This script is attached to the player's prefab.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameEvents.OnLobbyJoined += ScaleNetworkPlayer;

        var myID = transform.GetComponent<NetworkObject>().NetworkObjectId;
        if (IsOwnedByServer)
        {
            transform.name = "Host:" + myID;    //this must be the host
            DebugConsole.Log("Host:" + myID);
        }
        else
        {
            transform.name = "Client:" + myID; //this must be the client 
            DebugConsole.Log("Client:" + myID);
        }
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameEvents.OnLobbyJoined -= ScaleNetworkPlayer;
    }

    private void ScaleNetworkPlayer()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    void Update()
    {
        if (IsOwner)
        {
            root.position = OVRCameraRigReferencesForNetCode.Singleton.root.position;
            root.rotation = OVRCameraRigReferencesForNetCode.Singleton.root.rotation;

            head.position = OVRCameraRigReferencesForNetCode.Singleton.head.position;
            head.rotation = OVRCameraRigReferencesForNetCode.Singleton.head.rotation;

            leftHand.position = OVRCameraRigReferencesForNetCode.Singleton.leftHand.position;
            leftHand.rotation = OVRCameraRigReferencesForNetCode.Singleton.leftHand.rotation;

            rightHand.position = OVRCameraRigReferencesForNetCode.Singleton.rightHand.position;
            rightHand.rotation = OVRCameraRigReferencesForNetCode.Singleton.rightHand.rotation;
        }

    }
}
