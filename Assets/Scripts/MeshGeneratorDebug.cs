using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.UI;

public class MeshGeneratorDebug : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Transform quadTopLeft;
    [SerializeField] Transform quadTopRight;
    [SerializeField] Transform quadBotLeft;
    [SerializeField] Transform quadBotRight;

    [SerializeField] BoxCollider cube;

    void Start()
    {
        CreateCube();
    }

    [MethodButton("Create Triangle", true)]
    void CreateTriangle()
    {
        MeshGeneration.CreateTriangle(material);
    }
    
    [MethodButton("Create Quad", true)]
    void CreateQuad()
    {
        MeshGeneration.CreateQuad(new MeshGeneration.QuadFace(quadTopLeft.position, quadTopRight.position, quadBotLeft.position, quadBotRight.position), material);
    }
    
    [MethodButton("Create Cube", true)]
    void CreateCube()
    {
        MeshGeneration.CreateCube(cube.transform.position, cube.size / 2, cube.transform.rotation, material);
    }
}