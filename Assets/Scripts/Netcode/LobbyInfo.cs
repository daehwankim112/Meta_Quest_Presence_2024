using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfo : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText;
    
    string _joinCode = string.Empty;
    string _lobbyName = string.Empty;

    public void Initialize(Lobby lobby)
    {
        _joinCode = NetworkConnect.GetJoinCode(lobby);
        _lobbyName = NetworkConnect.GetLobbyName(lobby);
        
        lobbyNameText.SetText($"Lobby Name: {_lobbyName} \nJoin Code: {_joinCode} \nHosted By: {NetworkManager.Singleton.LocalClientId}" );
    }
}
