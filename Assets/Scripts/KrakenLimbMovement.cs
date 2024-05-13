using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class KrakenLimbMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] RandomTimer forceInterval;
    
    [SerializeField] float minForce, maxForce;

    void FixedUpdate()
    {
        if (!forceInterval.Complete()) return;

        float force = Random.Range(minForce, maxForce);
        rb.AddForce(Random.insideUnitSphere.normalized * force, ForceMode.VelocityChange);
    }
}
