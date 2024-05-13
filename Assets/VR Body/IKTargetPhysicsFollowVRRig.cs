using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetPhysicsFollowVRRig : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    [SerializeField] Rigidbody rb;

    public Vector3 headBodyPositionOffset;

    void FixedUpdate()
    {
        rb.MovePosition(head.vrTarget.position + headBodyPositionOffset);
        rb.MoveRotation( Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, head.vrTarget.eulerAngles.y, transform.eulerAngles.z),turnSmoothness));

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
