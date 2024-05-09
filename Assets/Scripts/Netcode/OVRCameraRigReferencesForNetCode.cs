// David Kim 2024/4/26
// Description: This is a script that references the OVRCameraRig for Netcode. This script is used to reference the OVRCameraRig for Netcode.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class OVRCameraRigReferencesForNetCode : MonoBehaviour
{
    public static OVRCameraRigReferencesForNetCode instance;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    void OnEnable()
    {
        instance = this;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
