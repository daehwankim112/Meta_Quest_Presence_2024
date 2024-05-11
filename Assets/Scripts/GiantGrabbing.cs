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
        _leftHand = new Hand();
        _rightHand = new Hand();
    }

    void Grab(HandSide handSide)
    {
        float grabRadiusMult = RoomEnvironmentInitializer.RoomScale.magnitude;
        switch (handSide)
        {
            case HandSide.Left:
                _leftHand.Grab(leftHandCollider.transform.position, leftHandCollider.radius * grabRadiusMult);
                break;
            case HandSide.Right:
                _rightHand.Grab(rightHandCollider.transform.position, rightHandCollider.radius * grabRadiusMult);
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
        if(_leftHand.IsHoldingObject) _leftHand.Grabbing(leftHandCollider.transform.position);
        if(_rightHand.IsHoldingObject) _rightHand.Grabbing(rightHandCollider.transform.position);
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
    