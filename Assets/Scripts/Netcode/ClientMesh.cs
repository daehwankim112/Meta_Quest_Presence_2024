using System.Collections.Generic;
using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Recieves mesh data from the host to create the mesh
/// </summary>
public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Tree treePrefab;
    
    NetworkList<Vector3> _vertices;
    NetworkList<Vector3> _normals;
    NetworkList<Vector2> _uvs;
    NetworkList<int> _triangles;
    NetworkList<Vector3> _treePositions;
    NetworkList<Quaternion> _treeRotations;

    void Awake()
    {
        _vertices = new NetworkList<Vector3>();
        _normals = new NetworkList<Vector3>();
        _triangles = new NetworkList<int>();
        _uvs = new NetworkList<Vector2>();
        _treePositions = new NetworkList<Vector3>();
        _treeRotations = new NetworkList<Quaternion>();
        
        _vertices.Initialize(this);
        _normals.Initialize(this);
        _triangles.Initialize(this);
        _uvs.Initialize(this);
        _treePositions.Initialize(this);
        _treeRotations.Initialize(this);
    }

    public void Initialize(Mesh mesh, List<Vector3> treePositions, List<Quaternion> treeRotations)
    {
        foreach (var v in mesh.vertices) _vertices.Add(v);
        foreach (var n in mesh.normals) _normals.Add(n);
        foreach (var t in mesh.triangles) _triangles.Add(t);
        foreach (var u in mesh.uv) _uvs.Add(u);
        foreach (var p in treePositions) _treePositions.Add(p);
        foreach (var r in treeRotations) _treeRotations.Add(r);
    }

    public override void OnNetworkSpawn()
    {
        CreateMesh();
    }

    void CreateMesh()
    {
        var vL = new Vector3[_vertices.Count];
        var nL = new Vector3[_normals.Count];
        var tL = new int[_triangles.Count];
        var uL = new Vector2[_uvs.Count];
        var tpL = new Vector3[_treePositions.Count];
        var trL = new Quaternion[_treeRotations.Count];

        for (int i = 0; i < _vertices.Count; i++) vL[i] = _vertices[i];
        for (int i = 0; i < _normals.Count; i++) nL[i] = _normals[i];
        for (int i = 0; i < _triangles.Count; i++) tL[i] = _triangles[i];
        for (int i = 0; i < _uvs.Count; i++) uL[i] = _uvs[i];
        for (int i = 0; i < _treePositions.Count; i++) tpL[i] = _treePositions[i];
        for (int i = 0; i < _treeRotations.Count; i++) trL[i] = _treeRotations[i];

        Mesh mesh = new();
        mesh.SetVertices(vL);
        mesh.SetNormals(nL);
        mesh.triangles = tL;
        mesh.uv = uL;

        for (int i = 0; i < tpL.Length; i++)
        {
            Instantiate(treePrefab, tpL[i], trL[i], meshFilter.transform);
        }
        
        meshFilter.mesh = mesh;
        meshFilter.transform.localScale = RoomEnvironmentInitializer.RoomScale;

        Vector3 spawnPosition = _treePositions[Random.Range(0, _treePositions.Count)].Add(y: 0.5f);
        GameEvents.InvokeRecievedSceneMeshFromHost(spawnPosition);
    }
}
