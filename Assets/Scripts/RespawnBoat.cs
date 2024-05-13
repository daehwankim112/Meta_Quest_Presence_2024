using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

public class RespawnBoat : NetworkBehaviour
{
    public Vector3 PlayerSpawnPos => playerSpawnPos.position;

    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerSpawnPos;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    public Vector3 destination;
    
    void Update()
    {
        if (!IsServer) return;
        
        Vector3 dirToDestination = VectorUtils.Direction(transform.position.With(y:0), destination.With(y:0));
        transform.rotation = Quaternion.LookRotation(dirToDestination.With(y:0));
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        
        rb.angularVelocity = Vector3.zero;
        Vector3 dirToDestination = VectorUtils.Direction(transform.position.With(y:0), destination.With(y:0));
        rb.velocity = dirToDestination * moveSpeed;
    }

    public bool ReachedDestination()
    {
        return Vector3.Distance(transform.position.With(y: 0), destination.With(y: 0)) <= 5f;
    }
}