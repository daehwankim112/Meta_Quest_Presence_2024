using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;

public class VivoxVoiceChat : NetworkBehaviour
{
    Channel3DProperties channel3DProperties;
    ulong clientID;
    string channelName;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            channel3DProperties = new Channel3DProperties();
            JoinPositionalChannelAsync();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            LogoutOfVivoxAsync();
        }
    }

    async void InitializeAsync()
    {
        // These are already handled in NetworkConnect.cs
        // await UnityServices.InitializeAsync();
        // await AuthenticationService.Instance.SignInAnonymouslyAsync();
        // await VivoxService.Instance.InitializeAsync();
    }

    public async void LoginToVicoxAsync()
    {
        LoginOptions options = new LoginOptions();
        options.DisplayName = "Client: " + OwnerClientId;
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync(options);
    }

    public async void JoinEchoChannelAsync()
    {
        channelName = NetworkConnect.instance.lobbyCode;
        await VivoxService.Instance.JoinEchoChannelAsync(channelName, ChatCapability.AudioOnly);
        Debug.LogError("VivoxService.Instance.IsLoggedIn: " + VivoxService.Instance.IsLoggedIn);
    }

    public async void JoinPositionalChannelAsync()
    {
        channelName = NetworkConnect.instance.lobbyCode;
        Debug.LogError("channel to join: " + channelName.Trim().Replace(" ", ""));
        await VivoxService.Instance.JoinPositionalChannelAsync(channelName.Trim().Replace(" ", ""), ChatCapability.AudioOnly, channel3DProperties);
        Debug.LogError("VivoxService.Instance.IsLoggedIn: " + VivoxService.Instance.IsLoggedIn);
        Debug.LogError("VivoxService.Instance.SignedInPlayerId: " + VivoxService.Instance.SignedInPlayerId);
    }

    public async void LogoutOfVivoxAsync()
    {
        channelName = NetworkConnect.instance.lobbyCode;
        await VivoxService.Instance.LeaveChannelAsync(channelName.Trim().Replace(" ", ""));
        await VivoxService.Instance.LogoutAsync();
        Debug.LogError("VivoxService signed out: " + channelName);
    }

    public async void LoginToVivoxAsync()
    {
        if (IsOwner)
        {
            LoginOptions loginOptions = new LoginOptions();
            loginOptions.DisplayName = "Client: " + clientID;
            await VivoxService.Instance.LoginAsync(loginOptions);
            join3DChannelAsync();
        }
    }

    public async void join3DChannelAsync()
    {
        await VivoxService.Instance.JoinEchoChannelAsync("3DChannel", ChatCapability.AudioOnly);
        Debug.LogError("Joined Echo Channel");
    }
}
