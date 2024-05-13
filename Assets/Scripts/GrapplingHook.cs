using System.Collections;
using System.Collections.Generic;
using NuiN.Movement;
using NuiN.NExtensions;
using NuiN.ScriptableHarmony.Sound;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    bool _attached;

    [SerializeField] SimpleTimer pullTimer;

    [SerializeField] Transform hand;
    
    [SerializeField] float maxAttachDistance = 25f;
    
    [SerializeField] InputActionProperty activateAction;
    [SerializeField] LayerMask attachableLayers;

    [SerializeField] LineRenderer indicatorLR;
    [SerializeField] LineRenderer ropeLR;

    [SerializeField] float initialForce = 10f;
    [SerializeField] float initialVelocityFalloff = 0.5f;
    [SerializeField] float pullForce = 5f;
    [SerializeField] Rigidbody rb;

    [SerializeField] SoundSO pullSound;
    [SerializeField] Transform headPos;

    [SerializeField] GroundMovementController movement;

    [SerializeField] Transform hitIndicator;
    [SerializeField] float grappleDrag = 0.5f;

    Transform _connectedTransform;
    Vector3 _localPos;
    bool _beingGrabbed;
    
    void Awake() => activateAction.action.Enable();
    void Start() => ropeLR.enabled = false;
    void OnEnable()
    {
        GameEvents.OnLocalClientGrabbed += SetBeingGrabbed;
        GameEvents.OnSmallPlayerFellInWater += DetachHandler;
        GameEvents.OnLocalClientReleased += SetReleased;
    }
    void OnDisable()
    {
        GameEvents.OnLocalClientGrabbed -= SetBeingGrabbed;
        GameEvents.OnLocalClientReleased -= SetReleased;
        GameEvents.OnSmallPlayerFellInWater -= DetachHandler;
    }

    void SetBeingGrabbed()
    {
        _beingGrabbed = true;
        Detach();
    }

    void DetachHandler(ulong _)
    {
        _beingGrabbed = false;
        Detach();
    }

    void SetReleased(Vector3 _)
    {
        _beingGrabbed = false;
        movement.EnableGravity();
        movement.EnableMovementNoKinematic();
    }

    void Update()
    {
        if (_beingGrabbed) return;
        
        if (activateAction.action.WasPressedThisFrame()) Activate();
        else if(activateAction.action.WasReleasedThisFrame()) Detach();

        if (!_attached && Hit(out RaycastHit hit))
        {
            hitIndicator.gameObject.SetActive(true);
            hitIndicator.position = hit.point;
        }
        else
        {
            hitIndicator.gameObject.SetActive(false);
        }

        if (_attached)
        {
            GameEvents.InvokeLocalPlayerGrappling(_connectedTransform.TransformPoint(_localPos));
        }
    }

    void LateUpdate()
    {
        if (!_attached)
        {
            indicatorLR.SetPosition(0, hand.position);
            indicatorLR.SetPosition(1, hand.position + hand.forward * maxAttachDistance);
        }
        else
        {
            ropeLR.SetPosition(0, hand.position);
            ropeLR.SetPosition(1, _connectedTransform.TransformPoint(_localPos));
        }
    }

    void FixedUpdate()
    {
        if (_attached)
        {
            Vector3 target = _connectedTransform.TransformPoint(_localPos);
            Vector3 dirToPos = VectorUtils.Direction(hand.position, target);
            rb.AddForce(dirToPos * pullForce, ForceMode.Acceleration);
        }
    }

    void Activate()
    {
        if (!Hit(out RaycastHit hit)) return;

        movement.DisableGravity();
        movement.DisableMovementNoKinematic();
        
        _connectedTransform = hit.collider.transform;
        _localPos = _connectedTransform.InverseTransformPoint(hit.point);

        rb.velocity *= initialVelocityFalloff;
        rb.AddForce(VectorUtils.Direction(hand.position, hit.point, initialForce), ForceMode.Impulse);


        ropeLR.enabled = true;
        ropeLR.positionCount = 2;
        
        indicatorLR.enabled = false;
        _attached = true;

        rb.drag = grappleDrag;
        
        pullSound?.PlaySpatial(hand.position);
    }

    void Detach()
    {
        rb.drag = 0;

        if (!_beingGrabbed)
        {
            movement.EnableGravity();
            movement.EnableMovementNoKinematic();
        }
        
        GameEvents.InvokeLocalPlayerUnGrappled();
        
        _connectedTransform = null;
        _localPos = Vector3.zero;

        ropeLR.enabled = false;
        indicatorLR.enabled = true;

        _attached = false;
    }

    bool Hit(out RaycastHit hit)
    {
        return Physics.Raycast(hand.position, hand.forward, out hit, maxAttachDistance, attachableLayers);
    }
}
