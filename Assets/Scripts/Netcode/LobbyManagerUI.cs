using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    [SerializeField] LobbyButton lobbyButtonPrefab;
    [SerializeField] TMP_InputField lobbyNameInput;
    
    List<LobbyButton> _lobbies = new();

    bool _destroyed;

    void OnEnable()
    {
        createLobbyButton.onClick.AddListener(CreateLobby);
        NetworkConnect.LobbyDeleted += ClearLobbies;
    }
    void OnDisable()
    {
        createLobbyButton.onClick.RemoveAllListeners();
        NetworkConnect.LobbyDeleted -= ClearLobbies;
    }

    void CreateLobby()
    {
        string inputText = lobbyNameInput.text.Trim();
        string lobbyName = inputText.Length > 0 ? inputText : DEFAULT_LOBBY_NAME;
        NetworkConnect.Create(lobbyName);
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
                LobbyButton lobbyButton = Instantiate(lobbyButtonPrefab, grid.transform);
                lobbyButton.Initialize(lobby);
                _lobbies.Add(lobbyButton);
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

    void OnDestroy()
    {
        _destroyed = true;
    }
}
