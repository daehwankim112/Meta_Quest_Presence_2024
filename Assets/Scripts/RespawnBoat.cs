using NuiN.NExtensions;
using UnityEngine;

public class RespawnBoat : MonoBehaviour
{
    public Vector3 PlayerSpawnPos => playerSpawnPos.position;

    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerSpawnPos;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    
    Vector3 _destination;
    
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }
    
    void Update()
    {
        Vector3 dirToDestination = VectorUtils.Direction(transform.position.With(y:0), _destination.With(y:0));
        transform.rotation = Quaternion.LookRotation(dirToDestination.With(y:0));
    }

    void FixedUpdate()
    {
        Vector3 dirToDestination = VectorUtils.Direction(transform.position, _destination);
        rb.velocity = dirToDestination * moveSpeed;
    }

    public bool ReachedDestination()
    {
        return Vector3.Distance(transform.position.With(y: 0), _destination.With(y: 0)) <= 5f;
    }
}