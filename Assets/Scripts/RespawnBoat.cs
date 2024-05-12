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

    public NetworkVariable<Vector3> destination = new();

    [SerializeField] NetworkObject networkObject;
    public NetworkObject NetworkObj => networkObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        destination.Initialize(this);
    }
    
    void Update()
    {
        Vector3 dirToDestination = VectorUtils.Direction(transform.position.With(y:0), destination.Value.With(y:0));
        transform.rotation = Quaternion.LookRotation(dirToDestination.With(y:0));
    }

    void FixedUpdate()
    {
        Vector3 dirToDestination = VectorUtils.Direction(transform.position, destination.Value);
        rb.velocity = dirToDestination * moveSpeed;
    }

    public bool ReachedDestination()
    {
        return Vector3.Distance(transform.position.With(y: 0), destination.Value.With(y: 0)) <= 5f;
    }
}