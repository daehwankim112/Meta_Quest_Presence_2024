// David Kim 2024/4/26
// Description: 
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
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
        }

    }
}
