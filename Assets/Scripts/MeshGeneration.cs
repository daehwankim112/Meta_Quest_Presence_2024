using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGeneration
{
    public static void CreateTriangle()
    {
        (Mesh mesh, MeshFilter meshFilter) = CreateMesh("Triangle");

        // generate vertices
        Vector3[] vertices = new [] {
            // generating three vertices for triangle
            new Vector3(0, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0),
        };

        mesh.vertices = vertices;
        
        // generate uv
        Vector2[] uv = new [] {
            // generated vertices will be mapped to corresponding coordinates in uv
            new Vector2(0.5f, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };
        mesh.uv = uv;
        
        Vector3[] normals = new [] {
            // pointing normal at (0,0,-1) i.e perpendicular to triangle we will be creating
            // flipping normals gives incorrect lighting
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };
        mesh.normals = normals;
        
        int[] triangles = new [] {
            // vertices index should be in clockwise order.
            // if it is in anticlockwise order mesh will be invisible from that direction
  
            0,1,2// index of vertices for creating triangle
        };
        mesh.triangles = triangles;
        
        meshFilter.mesh = mesh;
    }

    public static void CreateQuad(Vector3 topLeft, Vector3 topRight, Vector3 botLeft, Vector3 botRight)
    {
        (Mesh mesh, MeshFilter meshFilter) = CreateMesh("Quad");
        
        Vector3[] vertices = new [] {
            // creating vertices of quad. aligning them in shape of square
            topLeft,
            topRight,
            botLeft,
            botRight
        };
        mesh.vertices = vertices;
        
// generate uv
        Vector2[] uv = new [] {
            // generate uv for corresponding vertices also in form of square
    
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1),
        };
        mesh.uv = uv;

        Vector3[] normals = new [] {
            // normals same as tris
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };
        mesh.normals = normals;

        int[] triangles = new [] {
            // tris are viewed as group of three
            // remember to order them in clockwise
            // position of index is not importaint as long as they are in clockwise order
    
            0,1,2,// first tris
            2,1,3// second tris
        };
        mesh.triangles = triangles;
        
        meshFilter.mesh = mesh;
    }

    public static void CreateCube(Vector3 position, Vector3 extents, Vector3 size)
    {
        (Mesh mesh, MeshFilter meshFilter) = CreateMesh("Quad");
    }

    static (Mesh mesh, MeshFilter meshFilter) CreateMesh(string name)
    {
        Mesh mesh = new Mesh();
        GameObject obj = new GameObject(name);
        obj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();

        return (mesh, meshFilter);
    }
}