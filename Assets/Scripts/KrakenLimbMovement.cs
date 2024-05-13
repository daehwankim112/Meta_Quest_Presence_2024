using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class KrakenLimbMovement : MonoBehaviour
{
    [SerializeField] RandomTimer forceInterval;
    
    [SerializeField] float minForce, maxForce;
    
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!forceInterval.Complete()) return;

        float force = Random.Range(minForce, maxForce);
        _rb.AddForce(Random.insideUnitSphere.normalized * force, ForceMode.VelocityChange);
    }
}
