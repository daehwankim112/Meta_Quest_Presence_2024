using NuiN.NExtensions;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] GameObject giantPlayer;
    [SerializeField] GameObject smallPlayer;
    
    void Start()
    {
        SetPlayerAsGiant();
    }

    void OnEnable()
    {
        GameEvents.OnRecievedSceneMeshFromHost += SetPlayerAsSmall;
        GameEvents.OnLobbyHosted += SetPlayerAsGiant;
    }
    void OnDisable()
    {
        GameEvents.OnRecievedSceneMeshFromHost -= SetPlayerAsSmall;
        GameEvents.OnLobbyHosted -= SetPlayerAsGiant;
    }

    void SetPlayerAsGiant()
    {
        giantPlayer.transform.position = Vector3.zero;
        
        giantPlayer.SetActive(true);
        smallPlayer.SetActive(false);
    }
    
    void SetPlayerAsSmall(Vector3 spawnPosition)
    {
        smallPlayer.transform.position = spawnPosition;
        
        giantPlayer.SetActive(false);
        smallPlayer.SetActive(true);
    }
}
