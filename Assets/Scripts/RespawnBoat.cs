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
        Vector3 dirToDestination = VectorUtils.Direction(transform.position, _destination);
        Quaternion rotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(dirToDestination.With(y:0));
        rotation = Quaternion.RotateTowards(rotation, targetRotation, rotateSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    void FixedUpdate()
    {
        Vector3 dirToDestination = VectorUtils.Direction(transform.position, _destination);
        rb.AddForce(dirToDestination * moveSpeed, ForceMode.Acceleration);
    }

    public bool ReachedDestination()
    {
        return Vector3.Distance(transform.position.With(y: 0), _destination.With(y: 0)) <= 5f;
    }
}