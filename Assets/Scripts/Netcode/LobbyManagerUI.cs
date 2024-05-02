using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] GridLayoutGroup grid;

    [SerializeField] LobbyButton lobbyButtonPrefab;
    
    List<LobbyButton> _lobbies = new();

    
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
                lobbyButton.Initialize(NetworkConnect.GetJoinCode(lobby));
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
