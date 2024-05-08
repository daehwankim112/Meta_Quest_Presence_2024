using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostInitializer : MonoBehaviour
{
    [SerializeField] float scale = 0.1f;
    [SerializeField] Rigidbody host;

    void Awake()
    {
        host.isKinematic = true;
    }

    void OnEnable()
    {
        GameEvents.OnSceneMeshInitialized += ReleasePlayer;
    }
    void OnDisable()
    {
        GameEvents.OnSceneMeshInitialized -= ReleasePlayer;
    }

    void ReleasePlayer(MeshFilter meshFilter, List<Vector3> _1, List<Quaternion> _2)
    {
        host.transform.localScale = new Vector3(scale, scale, scale);
        host.isKinematic = false;
        host.velocity = Vector3.zero;
    }
}
