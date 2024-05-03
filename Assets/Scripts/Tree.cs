using NuiN.NExtensions;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] Transform sphereCheck;
    [SerializeField] Transform[] trunkCorners;
    [SerializeField] float trunkCornerCheckDist = 0.1f;
    [SerializeField] float sphereCheckRadius = 0.1f;

    public bool ValidPlacement(Vector3 instantiationPoint)
    {
        if (Physics.OverlapSphere(instantiationPoint + sphereCheck.position, sphereCheckRadius).Length > 0) return false;
        
        foreach (var corner in trunkCorners)
        {
            if (!Physics.Raycast(instantiationPoint + corner.position, Vector3.down * trunkCornerCheckDist)) return false;
        }
        
        return !Physics.Linecast(instantiationPoint.Add(y: top.position.y), instantiationPoint.Add(y: bottom.position.y));
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        
        if (top != null && bottom != null)
        {
            Gizmos.DrawLine(top.position, bottom.position);
        }

        foreach (var corner in trunkCorners)
        {
            if(corner == null) continue;
            Gizmos.DrawRay(corner.position, Vector3.down * trunkCornerCheckDist);
        }

        if (sphereCheck != null)
        {
            Gizmos.DrawWireSphere(sphereCheck.position, sphereCheckRadius);
        }
    }
}