using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] OVRCameraRigReferencesForNetCode giantPlayer;
    [SerializeField] OVRCameraRigReferencesForNetCode smallPlayer;

    [SerializeField] float spawnHeight = 10f;

    void Awake()
    {
        OVRCameraRigReferencesForNetCode.instance = giantPlayer;
    }

    void Start()
    {
        SetPlayerAsGiant();
    }

    void OnEnable()
    {
        GameEvents.OnRecievedSceneMeshFromHost += SetPlayerAsSmall;
        GameEvents.OnLobbyHosted += SetPlayerAsGiant;
        GameEvents.OnRoomScaled += ScaleGiant;
    }
    void OnDisable()
    {
        GameEvents.OnRecievedSceneMeshFromHost -= SetPlayerAsSmall;
        GameEvents.OnLobbyHosted -= SetPlayerAsGiant;
        GameEvents.OnRoomScaled -= ScaleGiant;
    }

    void SetPlayerAsGiant()
    {
        OVRCameraRigReferencesForNetCode.instance = giantPlayer;
        giantPlayer.transform.position = Vector3.zero;
        
        giantPlayer.gameObject.SetActive(true);
        smallPlayer.gameObject.SetActive(false);
    }
    
    void SetPlayerAsSmall(Vector3 spawnPosition)
    {
        OVRCameraRigReferencesForNetCode.instance = smallPlayer;

        smallPlayer.transform.position = spawnPosition.Add(y: spawnHeight);
        
        giantPlayer.gameObject.SetActive(false);
        smallPlayer.gameObject.SetActive(true);
    }

    void ScaleGiant()
    {
        giantPlayer.transform.localScale = RoomEnvironmentInitializer.RoomScale;
    }
}
