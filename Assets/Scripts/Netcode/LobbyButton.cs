using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] Button button;

    string _joinCode = string.Empty;
    string _lobbyName = string.Empty;

    void OnEnable()
    {
        button.onClick.AddListener(JoinLobby);
    }
    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void Initialize(Lobby lobby)
    {
        _joinCode = NetworkConnect.GetJoinCode(lobby);
        _lobbyName = NetworkConnect.GetLobbyName(lobby);

        lobbyNameText.SetText(_lobbyName);
    }

    void JoinLobby()
    {
        NetworkConnect.Join(_joinCode);
    }
}
