using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.UI;

public class MeshGeneratorDebug : MonoBehaviour
{
    [SerializeField] Transform quadTopLeft;
    [SerializeField] Transform quadTopRight;
    [SerializeField] Transform quadBotLeft;
    [SerializeField] Transform quadBotRight;
    
    [MethodButton("Create Triangle", true)]
    void CreateTriangle()
    {
        MeshGeneration.CreateTriangle();
    }
    
    [MethodButton("Create Quad", true)]
    void CreateQuad()
    {
        MeshGeneration.CreateQuad(quadTopLeft.position, quadTopRight.position, quadBotLeft.position, quadBotRight.position);
    }
}