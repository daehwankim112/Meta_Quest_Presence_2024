using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class KrakenLimbMovement : MonoBehaviour
{
    [SerializeField] RandomTimer forceInterval;
    
    [SerializeField] float minForce, maxForce;
    
    Rigidbody _rb;
    Collider _col;

    [SerializeField] Rigidbody[] rbsToSolve;

    [SerializeField] Transform root;
    
    [SerializeField] float solverDistance = 20f;
    [SerializeField] float correctionSpeed = 2f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        float distFromRoot = Vector3.Distance(_rb.position, root.position);
        if (distFromRoot > solverDistance)
        {
            foreach (var rb in rbsToSolve)
            {
                rb.MovePosition(rb.position + VectorUtils.Direction(rb.position, root.position) * correctionSpeed);
            }
        }
        
        if (!forceInterval.Complete()) return;

        float force = Random.Range(minForce, maxForce);
        _rb.AddForce(Random.insideUnitSphere.normalized * force, ForceMode.VelocityChange);
    }
}
