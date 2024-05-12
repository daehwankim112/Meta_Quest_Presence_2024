using NuiN.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiantGrabbing : MonoBehaviour
{
    [SerializeField] Transform leftHandTransform;
    [SerializeField] Transform rightHandTransform;

    [SerializeField] InputActionProperty leftHandGrabAction;
    [SerializeField] InputActionProperty rightHandGrabAction;

    [SerializeField] float grabRadius;
    [SerializeField] float grabZOffset;

    Vector3 LeftHandPosition => leftHandTransform.position + leftHandTransform.forward * grabZOffset;
    Vector3 RightHandPosition => rightHandTransform.position + rightHandTransform.forward * grabZOffset;

    Hand _leftHand, _rightHand;

    enum HandSide
    {
        Left,
        Right
    }

    void OnEnable()
    {
        leftHandGrabAction.action.performed += LeftGrab;
        leftHandGrabAction.action.canceled += LeftRelease;
        rightHandGrabAction.action.performed += RightGrab;
        rightHandGrabAction.action.canceled += RightRelease;
    }
    
    void OnDisable()
    {
        leftHandGrabAction.action.performed -= LeftGrab;
        leftHandGrabAction.action.canceled -= LeftRelease;
        rightHandGrabAction.action.performed -= RightGrab;
        rightHandGrabAction.action.canceled -= RightRelease;
    }
    
    void Start()
    {
        leftHandGrabAction.Enable();
        rightHandGrabAction.Enable();
        _leftHand = new Hand();
        _rightHand = new Hand();
    }

    void Grab(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Left:
                _leftHand.Grab(LeftHandPosition, grabRadius);
                break;
            case HandSide.Right:
                _rightHand.Grab(RightHandPosition, grabRadius);
                break;
        }
    }
    
    void Release(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Left:
                _leftHand.Release();
                break;
            case HandSide.Right:
                _rightHand.Release();
                break;
        }
    }

    void Update()
    {
        if(_leftHand.IsHoldingObject) _leftHand.Grabbing(LeftHandPosition);
        if(_rightHand.IsHoldingObject) _rightHand.Grabbing(RightHandPosition);
    }

    void LeftRelease(InputAction.CallbackContext context)
    {
        Release(HandSide.Left);
    }


    void LeftGrab(InputAction.CallbackContext context)
    {
        Grab(HandSide.Left);
    }

    void RightRelease(InputAction.CallbackContext context)
    {
        Release(HandSide.Right);
    }

    void RightGrab(InputAction.CallbackContext context)
    {
        Grab(HandSide.Right);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(LeftHandPosition, grabRadius);
        Gizmos.DrawWireSphere(RightHandPosition, grabRadius);
    }
}
    