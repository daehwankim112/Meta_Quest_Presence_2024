// David Kim 2024/4/26
// Description: Use Netcode to synchronize the player's head, hands, and root. This script is attached to the player's prefab.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private OVRSkeleton leftHandSkeleton;
    [SerializeField] private OVRSkeleton rightHandSkeleton;

    [SerializeField] private Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var myID = transform.GetComponent<NetworkObject>().NetworkObjectId;
        if (IsOwnedByServer)
        {
            transform.name = "Host:" + myID; //this must be the host
            DebugPanel.Send("Host:" + myID);
        }
        else
        {
            transform.name = "Client:" + myID; //this must be the client 
            DebugPanel.Send("Client:" + myID);
        }
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }

        // Left Hand debug console statements
        if (OVRCameraRigReferencesForNetCode.Singleton.leftOVRSkeleton.gameObject.GetComponent<OVRHand>().IsTracked)
        {
            DebugPanel.Send("Left Hand Tracked by " + transform.GetComponent<NetworkObject>().NetworkObjectId);
            DebugPanel.Send("Left Hand Confidence: " + OVRCameraRigReferencesForNetCode.Singleton.leftOVRHand.HandConfidence);
            DebugPanel.Send("Left Hand Scale: " + OVRCameraRigReferencesForNetCode.Singleton.leftOVRHand.HandScale);
            if (OVRCameraRigReferencesForNetCode.Singleton.leftOVRSkeleton.gameObject.GetComponent<OVRHand>().IsDataValid) {
                DebugPanel.Send("Left Hand Data Valid");
            }
        }

        // Right Hand debug console statements
        if (OVRCameraRigReferencesForNetCode.Singleton.rightOVRSkeleton.gameObject.GetComponent<OVRHand>().IsTracked)
        {
            DebugPanel.Send("Right Hand Tracked by " + transform.GetComponent<NetworkObject>().NetworkObjectId);
            DebugPanel.Send("Right Hand Confidence: " + OVRCameraRigReferencesForNetCode.Singleton.rightOVRHand.HandConfidence);
            DebugPanel.Send("Right Hand Scale: " + OVRCameraRigReferencesForNetCode.Singleton.rightOVRHand.HandScale);
            if (OVRCameraRigReferencesForNetCode.Singleton.rightOVRSkeleton.gameObject.GetComponent<OVRHand>().IsDataValid)
            {
                DebugPanel.Send("Right Hand Data Valid");
            }
        }
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

            UpdateSkeleton();
        }

    }

    private void UpdateSkeleton()
    {
        if (OVRCameraRigReferencesForNetCode.Singleton.leftHand)
        {
            if (OVRCameraRigReferencesForNetCode.Singleton.leftOVRSkeleton.gameObject.GetComponent<OVRHand>().IsDataValid)
            {
                // Scale the hand
                // leftHandSkeleton.gameObject.GetComponent<OVRHand>().HandScale = OVRCameraRigReferencesForNetCode.Singleton.leftHandSkeleton.gameObject.GetComponent<OVRHand>().HandScale;
                // Update position and location of the bones
                for (int i = 0; i < OVRCameraRigReferencesForNetCode.Singleton.leftOVRSkeleton.GetCurrentNumBones(); i++)
                {
                    leftHandSkeleton.Bones[i].Transform = OVRCameraRigReferencesForNetCode.Singleton.leftOVRSkeleton.Bones[i].Transform;
                }
            }

        }
        if (OVRCameraRigReferencesForNetCode.Singleton.rightHand)
        {
            if (OVRCameraRigReferencesForNetCode.Singleton.rightOVRSkeleton.gameObject.GetComponent<OVRHand>().IsDataValid)
            {
                // Scale the hand
                // rightHandSkeleton.gameObject.GetComponent<OVRHand>().HandScale = OVRCameraRigReferencesForNetCode.Singleton.rightHandSkeleton.gameObject.GetComponent<OVRHand>().HandScale;
                // Update position and location of the bones
                for (int i = 0; i < OVRCameraRigReferencesForNetCode.Singleton.rightOVRSkeleton.GetCurrentNumBones(); i++)
                {
                    rightHandSkeleton.Bones[i].Transform = OVRCameraRigReferencesForNetCode.Singleton.rightOVRSkeleton.Bones[i].Transform;
                }
            }
        }
    }
}
