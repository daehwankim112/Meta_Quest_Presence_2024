// David Kim 2024/4/26
// Description: This is a script that references the OVRCameraRig for Netcode. This script is used to reference the OVRCameraRig for Netcode.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class OVRCameraRigReferencesForNetCode : MonoBehaviour
{
    public static OVRCameraRigReferencesForNetCode Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}
