using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    const string DEFAULT_LOBBY_NAME = "New Lobby";

    [SerializeField] GameObject lobbiesPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] Button exitLobbyButton;

    [SerializeField] TMP_Text playerCountText;
    [SerializeField] Button createLobbyButton;
    [SerializeField] TMP_InputField lobbyNameInput;
    
    [SerializeField] GridLayoutGroup grid;

    [SerializeField] LobbyButton lobbyButtonPrefab;
    
    List<LobbyButton> _lobbies = new();

    void OnEnable()
    {
        exitLobbyButton.onClick.AddListener(ExitLobby);
        createLobbyButton.onClick.AddListener(CreateLobby);
        NetworkConnect.LobbyDeleted += ClearLobbies;
    }
    void OnDisable()
    {
        createLobbyButton.onClick.RemoveAllListeners();
        exitLobbyButton.onClick.RemoveAllListeners();
        NetworkConnect.LobbyDeleted -= ClearLobbies;
    }

    void CreateLobby()
    {
        string inputText = lobbyNameInput.text.Trim();
        string lobbyName = inputText.Length > 0 ? inputText : DEFAULT_LOBBY_NAME;
        NetworkConnect.Create(lobbyName);
        
        lobbiesPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        lobbyNameText.SetText(lobbyName);
    }

    void ExitLobby()
    {
        lobbiesPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        
        NetworkConnect.DeleteLobby();
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

            ClearLobbies();

            foreach (var lobby in foundLobbies.Results)
            {
                LobbyButton lobbyButton = Instantiate(lobbyButtonPrefab, grid.transform);
                lobbyButton.Initialize(lobby);
                _lobbies.Add(lobbyButton);
            }
            
            UpdatePlayerCount();

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

        playerCountText.text = "Players: 1";
    }

    void UpdatePlayerCount()
    {
        if (NetworkConnect.instance.CurrentLobby == null) return;
        
        playerCountText.text = $"Players: {NetworkManager.Singleton.ConnectedClientsList.Count}";
    }
}
