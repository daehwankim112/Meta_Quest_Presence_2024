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
    [SerializeField] Transform root;
    [SerializeField] Transform head;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    [SerializeField] Renderer[] meshToDisable;
    [SerializeField] Collider[] collidersToDestroy;

    [SerializeField] float smallPlayerInitialScale = 2f;
    [SerializeField] float giantInitialScale = 1f;
    
    [SerializeField] Transform grapplePoint;
    [SerializeField] LineRenderer grapplingLR;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        NetworkObject networkObj = GetComponent<NetworkObject>();
        var myID = networkObj.OwnerClientId;
        
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

            foreach (var col in collidersToDestroy)
            {
                Destroy(col);
            }

            float scale = IsServer ? giantInitialScale * RoomEnvironmentInitializer.RoomScale.magnitude: smallPlayerInitialScale;
            Vector3 scaleVector = Vector3.one * scale;
            
            head.localScale = scaleVector;
            leftHand.localScale = scaleVector;
            rightHand.localScale = scaleVector;

            if (!IsServer)
            {
                GameEvents.OnLocalPlayerGrappling += SetGrapplePosition;
                GameEvents.OnLocalPlayerUnGrappled += ResetGrapplePosition;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer)
        {
            GameEvents.OnLocalPlayerGrappling -= SetGrapplePosition;
            GameEvents.OnLocalPlayerUnGrappled -= ResetGrapplePosition;
        }
    }

    void SetGrapplePosition(Vector3 position)
    {
        grapplePoint.position = position;
    }

    void ResetGrapplePosition()
    {
        grapplePoint.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (IsOwner)
        {
            root.position = OVRCameraRigReferencesForNetCode.instance.root.position;
            root.rotation = OVRCameraRigReferencesForNetCode.instance.root.rotation;

            head.position = OVRCameraRigReferencesForNetCode.instance.head.position;
            head.rotation = OVRCameraRigReferencesForNetCode.instance.head.rotation;

            leftHand.position = OVRCameraRigReferencesForNetCode.instance.leftHand.position;
            leftHand.rotation = OVRCameraRigReferencesForNetCode.instance.leftHand.rotation;

            rightHand.position = OVRCameraRigReferencesForNetCode.instance.rightHand.position;
            rightHand.rotation = OVRCameraRigReferencesForNetCode.instance.rightHand.rotation;
        }
    }

    void LateUpdate()
    {
        if (IsOwner && !IsServer)
        {
            Debug.LogError("Local player, disabling lr");
            grapplingLR.enabled = false;
            return;
        }
        
        if (grapplePoint.localPosition != Vector3.zero)
        {
            grapplingLR.enabled = true;
            grapplingLR.SetPosition(0, rightHand.position);
            grapplingLR.SetPosition(1, grapplePoint.position);
        }
        else
        {
            grapplingLR.enabled = false;
        }
    }
}
