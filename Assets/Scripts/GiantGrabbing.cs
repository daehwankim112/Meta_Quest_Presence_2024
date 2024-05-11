using NuiN.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiantGrabbing : MonoBehaviour
{
    [SerializeField] SphereCollider leftHandCollider;
    [SerializeField] SphereCollider rightHandCollider;

    [SerializeField] InputActionProperty leftHandGrabAction;
    [SerializeField] InputActionProperty rightHandGrabAction;

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
        _leftHand = new Hand(leftHandCollider.radius);
        _rightHand = new Hand(rightHandCollider.radius);
    }

    void Grab(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Left:
                DebugConsole.Log("Left Hand Grabbing");
                _leftHand.Grab(leftHandCollider.transform.position);
                break;
            case HandSide.Right:
                DebugConsole.Log("Right Hand Grabbing");
                _rightHand.Grab(rightHandCollider.transform.position);
                break;
        }
    }
    
    void Release(HandSide handSide)
    {
        switch (handSide)
        {
            case HandSide.Left:
                DebugConsole.Log("Left Hand Releasing");
                break;
            case HandSide.Right:
                DebugConsole.Log("Right Hand Releasing");
                break;
        }
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
}
    