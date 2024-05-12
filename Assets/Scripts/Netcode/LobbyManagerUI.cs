using NuiN.NExtensions;
using NuiN.ScriptableHarmony.Sound;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    const string DEFAULT_LOBBY_NAME = "New Lobby";

    [SerializeField] GridLayoutGroup grid;
    [SerializeField] GameObject lobbiesPanel;
    [SerializeField] Button createLobbyButton;
    [SerializeField] Button exitLobbyButton;
    [SerializeField] GameObject lobbyButtonPrefab;
    [SerializeField] GameObject lobbyInfoPrefab;
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] SoundSO ExitSound;
    [SerializeField] SoundSO hostedSound;
    [SerializeField] SoundSO joinedSound;

    List<GameObject> _lobbies = new();

    bool _destroyed;

    void OnEnable()
    {
        createLobbyButton.onClick.AddListener(CreateLobby);
        exitLobbyButton.onClick.AddListener(ExitLobby);
        NetworkConnect.LobbyDeleted += ClearLobbies;
        GameEvents.OnLobbyHosted += PlayHostedSound;
        GameEvents.OnLobbyJoined += PlayJoinedSound;
    }


    void OnDisable()
    {
        createLobbyButton.onClick.RemoveAllListeners();
        exitLobbyButton.onClick.RemoveAllListeners();
        NetworkConnect.LobbyDeleted -= ClearLobbies;
        GameEvents.OnLobbyHosted -= PlayHostedSound;
        GameEvents.OnLobbyJoined -= PlayJoinedSound;
    }

    void CreateLobby()
    {
        string inputText = lobbyNameInput.text.Trim();
        string lobbyName = inputText.Length > 0 ? inputText : DEFAULT_LOBBY_NAME;
        NetworkConnect.Create(lobbyName);
        exitLobbyButton.gameObject.SetActive(true);
        createLobbyButton.gameObject.SetActive(false);
    }
    private void ExitLobby()
    {
        NetworkConnect.DeleteLobby();
        createLobbyButton.gameObject.SetActive(true);
        exitLobbyButton.gameObject.SetActive(false);
        ExitSound.Play();
    }

    async void Start()
    {
        while (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn)
        {
            await Task.Delay(1000);
        }
        
        PingLobbies();
    }

    async void PingLobbies()
    {
        try
        {
            if (_destroyed) return;
            
            var foundLobbies = await Lobbies.Instance.QueryLobbiesAsync();
            
            if (_destroyed) return;

            if (!Application.isPlaying) return;

            ClearLobbies();

            foreach (var lobby in foundLobbies.Results)
            {
                if (!NetworkManager.Singleton.IsServer)
                {
                    GameObject lobbyButton = Instantiate(lobbyButtonPrefab, grid.transform);
                    lobbyButton.GetComponent<LobbyButton>().Initialize(lobby);
                    _lobbies.Add(lobbyButton);
                }
                else
                {
                    GameObject lobbyInfo = Instantiate(lobbyInfoPrefab, grid.transform);
                    lobbyInfo.GetComponent<LobbyButton>().Initialize(lobby);
                    _lobbies.Add(lobbyInfo);
                }
            }
            
            await Task.Delay(2000);
            PingLobbies();
        }
        catch (Exception err)
        {
            Debug.LogWarning(err.Message);
        }
    }

    void ClearLobbies()
    {
        foreach (var oldLobby in _lobbies)
        {
            Destroy(oldLobby.gameObject);
        }

        _lobbies.Clear();
    }
    void PlayHostedSound()
    {
        RuntimeHelper.DoAfter(0.5f, () =>
        {
            hostedSound.Play();
        });
    }

    void PlayJoinedSound()
    {
        RuntimeHelper.DoAfter(0.1f, () =>
        {
            joinedSound.Play();
        });
    }

    void OnDestroy()
    {
        _destroyed = true;
    }
}
