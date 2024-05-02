using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    const string DEFAULT_LOBBY_NAME = "New Lobby";
    
    [SerializeField] Button createLobbyButton;
    [SerializeField] TMP_InputField lobbyNameInput;
    
    [SerializeField] GridLayoutGroup grid;

    [SerializeField] LobbyButton lobbyButtonPrefab;
    
    List<LobbyButton> _lobbies = new();

    void OnEnable()
    {
        createLobbyButton.onClick.AddListener(CreateLobby);
    }
    void OnDisable()
    {
        createLobbyButton.onClick.RemoveAllListeners();
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
            var foundLobbies = await Lobbies.Instance.QueryLobbiesAsync();

            if (!Application.isPlaying) return;
            
            foreach (var oldLobby in _lobbies)
            {
                Destroy(oldLobby.gameObject);
            }
            
            _lobbies.Clear();

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
}
