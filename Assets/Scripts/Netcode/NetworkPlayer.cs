// David Kim 2024/4/26
// Description: Use Netcode to synchronize the player's head, hands, and root. This script is attached to the player's prefab.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;
using NuiN.NExtensions;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] Transform head;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    [SerializeField] Renderer[] meshToDisable;
    [SerializeField] Collider[] collidersToDestroy;

    [SerializeField] float smallPlayerInitialScale;
    [SerializeField] float giantPlayerInitialScale;
    
    [SerializeField] Transform grapplePoint;
    [SerializeField] LineRenderer grapplingLR;

    [SerializeField] GameObject bodyVisual;
    [SerializeField] GameObject krakenVisual;

    Vector3 _lastRootPos;
    Vector3 _lastHeadPos;
    Vector3 _lastLeftHandPos;
    Vector3 _lastRightHandPos;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        NetworkObject networkObj = GetComponent<NetworkObject>();
        var myID = networkObj.OwnerClientId;
        
        if (IsOwnedByServer)
        {
            transform.name = "Host:" + myID;    //this must be the host
            DebugConsole.Log("Host:" + myID);
            
            bodyVisual.gameObject.SetActive(false);
            krakenVisual.gameObject.SetActive(true);
        }
        else
        {
            transform.name = "Client:" + myID; //this must be the client 
            DebugConsole.Log("Client:" + myID);
            
            bodyVisual.gameObject.SetActive(true);
            krakenVisual.gameObject.SetActive(false);
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

            float scale = IsServer ? giantPlayerInitialScale * RoomEnvironmentInitializer.RoomScale.magnitude : smallPlayerInitialScale;
            Vector3 scaleVector = Vector3.one * scale;
            if (IsServer) scale *= RoomEnvironmentInitializer.RoomScale.magnitude;
            
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
        RuntimeHelper.DoAfter(0.5f, GeneralUtils.ReloadScene);
        
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
            Vector3 rootPos = OVRCameraRigReferencesForNetCode.instance.root.position;
            if (rootPos != Vector3.zero)
            {
                _lastRootPos = rootPos;
            }
            root.position = _lastRootPos;
            root.rotation = OVRCameraRigReferencesForNetCode.instance.root.rotation;

            Vector3 headPos = OVRCameraRigReferencesForNetCode.instance.head.position;
            if (headPos != Vector3.zero)
            {
                _lastHeadPos = headPos;
            }
            head.position = _lastHeadPos;
            head.rotation = OVRCameraRigReferencesForNetCode.instance.head.rotation;

            Vector3 leftHandPos = OVRCameraRigReferencesForNetCode.instance.leftHand.position;
            if (leftHandPos != Vector3.zero)
            {
                _lastLeftHandPos = leftHandPos;
            }
            leftHand.position = _lastLeftHandPos;
            leftHand.rotation = OVRCameraRigReferencesForNetCode.instance.leftHand.rotation;

            Vector3 rightHandPos = OVRCameraRigReferencesForNetCode.instance.rightHand.position;
            if (rightHandPos != Vector3.zero)
            {
                _lastRightHandPos = rightHandPos;
            }
            rightHand.position = _lastRightHandPos;
            rightHand.rotation = OVRCameraRigReferencesForNetCode.instance.rightHand.rotation;
        }
    }

    void LateUpdate()
    {
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
