using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VoiceChat : NetworkBehaviour
{
    [SerializeField] Transform headPos;
    
    SimpleTimer _updateInterval = new(0.3f);
    string _channelName;

    bool _initialized;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer) return;
        Initialize();
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        VivoxService.Instance.LoggedIn -= LoggedInHandler;
        VivoxService.Instance.LoggedOut -= LoggedOutHandler;
    }

    async void Initialize()
    {
        await VivoxService.Instance.InitializeAsync();
        Debug.LogError("Vivox Initialized!");
        _initialized = true;

        VivoxService.Instance.LoggedIn += LoggedInHandler;
        VivoxService.Instance.LoggedOut += LoggedOutHandler;

        LoginAsync();
    }

    void LoggedInHandler()
    {
        Debug.LogError("Logged into Vivox: " + OwnerClientId);
    }

    void LoggedOutHandler()
    {
        VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.LogError("Left all Vivox voice channels: " + OwnerClientId);
        
        /*
        VivoxService.Instance.LogoutAsync();
        Debug.LogError("Logged out of Vivox: " + OwnerClientId);*/
    }

    async void LoginAsync()
    {
        LoginOptions options = new LoginOptions
        {
            DisplayName = "Client_" + OwnerClientId
        };
        await VivoxService.Instance.LoginAsync(options);
        
        Join3DChannelAsync();
    }

    void Update()
    {
        if (!VivoxService.Instance.IsLoggedIn || VivoxService.Instance.ActiveChannels.Count <= 0) return;
        
        if (!IsLocalPlayer) return;
        if (!_updateInterval.Complete()) return;
        
        UpdatePlayer3DPosition();
    }

    void UpdatePlayer3DPosition()
    {
        VivoxService.Instance.Set3DPosition(headPos.gameObject, _channelName);
        Debug.LogError("Transmitting Voice!");
    }

    async void Join3DChannelAsync()
    {
        _channelName = NetworkConnect.CurrentLobbyCode;

        int audibleDistance = IsHost ? 480 : 32;
        int conversationalDistance = IsHost ? 15 : 1;
        float audioFadeIntensityByDistanceaudio = 1.0f;
        AudioFadeModel audioFadeMode = AudioFadeModel.LinearByDistance;
        
        Channel3DProperties channelProperties = new Channel3DProperties(audibleDistance, conversationalDistance, audioFadeIntensityByDistanceaudio, audioFadeMode);
        await VivoxService.Instance.JoinPositionalChannelAsync(_channelName, ChatCapability.AudioOnly, channelProperties);
        
        Debug.LogError("Joined 3D voice Channel");
    }
}
