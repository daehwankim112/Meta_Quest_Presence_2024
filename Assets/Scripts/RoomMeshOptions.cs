using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMeshOptions : MonoBehaviour
{
    [SerializeField] Material roomMeshMaterial;
    
    public void SetRoomMeshMaterial(MeshFilter meshFilter)
    {
        if (roomMeshMaterial == null) return;
        
        meshFilter.GetComponent<MeshRenderer>().material = roomMeshMaterial;
    }
}
