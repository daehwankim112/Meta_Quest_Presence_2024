using UnityEngine;

public static class MeshGeneration
{
    public static void CreateTriangle(Material material)
    {
        (Mesh mesh, MeshFilter meshFilter) = CreateMesh("Triangle", material);

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

    public static GameObject CreateQuad(QuadFace quadFace, Material material, bool addCollder = true)
    {
        (Mesh mesh, MeshFilter meshFilter) = CreateMesh("Quad", material);
        GameObject obj = meshFilter.gameObject;
        
        Vector3[] vertices = new [] {
            // creating vertices of quad. aligning them in shape of square
            quadFace.topLeft,
            quadFace.topRight,
            quadFace.botLeft,
            quadFace.botRight
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

        if (addCollder)
        {
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
        }

        return obj;
    }

    public static GameObject CreateCube(Vector3 position, Vector3 extents, Quaternion rotation, Material material)
    {
        QuadFace topFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(-extents.x, extents.y, extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(extents.x, extents.y, extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(-extents.x, extents.y, -extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(extents.x, extents.y, -extents.z), position, rotation));
        
        QuadFace botFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(-extents.x, -extents.y, extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(extents.x, -extents.y, extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(-extents.x, -extents.y, -extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(extents.x, -extents.y, -extents.z), position, rotation));
        
        QuadFace leftFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(-extents.x, extents.y, extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(-extents.x, extents.y, -extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(-extents.x, -extents.y, extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(-extents.x, -extents.y, -extents.z), position, rotation));
        
        QuadFace rightFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(extents.x, extents.y, extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(extents.x, extents.y, -extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(extents.x, -extents.y, extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(extents.x, -extents.y, -extents.z), position, rotation));
        
        QuadFace frontFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(-extents.x, extents.y, extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(extents.x, extents.y, extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(-extents.x, -extents.y, extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(extents.x, -extents.y, extents.z), position, rotation));
        
        QuadFace backFace = new QuadFace(
            topLeft: PivotVector3(position + new Vector3(-extents.x, extents.y, -extents.z), position, rotation),
            topRight: PivotVector3(position + new Vector3(extents.x, extents.y, -extents.z), position, rotation),
            botLeft: PivotVector3(position + new Vector3(-extents.x, -extents.y, -extents.z), position, rotation),
            botRight: PivotVector3(position + new Vector3(extents.x, -extents.y, -extents.z), position, rotation));

        Transform cube = new GameObject("Cube").transform;
        cube.position = position;
        cube.rotation = rotation;
        CreateQuad(topFace, material).transform.SetParent(cube);
        CreateQuad(botFace, material).transform.SetParent(cube);
        CreateQuad(leftFace, material).transform.SetParent(cube);
        CreateQuad(rightFace, material).transform.SetParent(cube);
        CreateQuad(frontFace, material).transform.SetParent(cube);
        CreateQuad(backFace, material).transform.SetParent(cube);

        return cube.gameObject;
    }

    static (Mesh mesh, MeshFilter meshFilter) CreateMesh(string name, Material material)
    {
        Mesh mesh = new Mesh();
        GameObject obj = new GameObject(name);
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        
        meshRenderer.material = material;

        return (mesh, meshFilter);
    }

    public struct QuadFace
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 botLeft;
        public Vector3 botRight;

        public QuadFace(Vector3 topLeft, Vector3 topRight, Vector3 botLeft, Vector3 botRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.botLeft = botLeft;
            this.botRight = botRight;
        }
    }
    
    static Vector3 PivotVector3(Vector3 point, Vector3 pivot, Quaternion rotation) 
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(rotation.eulerAngles) * dir;
        point = dir + pivot;
        return point;
    }
}