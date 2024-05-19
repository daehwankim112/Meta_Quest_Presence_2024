using Oculus.VoiceSDK.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;

public class VivoxVoiceChat : NetworkBehaviour
{
    Channel3DProperties channel3DProperties;
    NetworkPlayer player;
    ulong clientID;
    bool isIn3DChannel;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            JoinEchoChannelAsync();
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
        string channelTojoin = "Lobby";
        await VivoxService.Instance.JoinEchoChannelAsync(channelTojoin, ChatCapability.AudioOnly);
        Debug.LogError("VivoxService.Instance.IsLoggedIn: " + VivoxService.Instance.IsLoggedIn);
        Debug.LogError("VivoxService.Instance.SignedInPlayerId: " + VivoxService.Instance.SignedInPlayerId);
    }

    public async void JoinPositionalChannelAsync()
    {
        string channelTojoin = "Lobby";
        await VivoxService.Instance.JoinPositionalChannelAsync(channelTojoin, ChatCapability.AudioOnly, channel3DProperties);
    }

    public async void LogoutOfVivoxAsync()
    {
        string channelToLeave = "Lobby";
        await VivoxService.Instance.LeaveChannelAsync(channelToLeave);
        await VivoxService.Instance.LogoutAsync();
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

    /*public void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText, bool transmissionSwitch = true, Channel3DProperties properties = null)
    {
        if (LoginSession.State == LoginState.LoggedIn)
        {
            Channel channel = new Channel(channelName, channelType, properties);

            IChannelSession channelSession = LoginSession.GetChannelSession(channel);

            channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar =>
            {
                try
                {
                    channelSession.EndConnect(ar);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not connect to channel: {e.Message}");
                    return;
                }
            });
        }
        else
        {
            Debug.LogError("Can't join a channel when not logged in.");
        }
    }

    public void Login(string displayName = null)
    {
        var account = new Account(displayName);
        bool connectAudio = true;
        bool connectText = true;

        LoginSession = VivoxService.Instance.Client.GetLoginSession(account);
        LoginSession.PropertyChanged += LoginSession_PropertyChanged;

        LoginSession.BeginLogin(LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
        {
            try
            {
                LoginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                // Unbind any login session-related events you might be subscribed to.
                // Handle error
                return;
            }
            // At this point, we have successfully requested to login. 
            // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
            // Reference LoginSession_PropertyChanged()
        });
    }

    // For this example, we immediately join a channel after LoginState changes to LoginState.LoggedIn.
    // In an actual game, when to join a channel will vary by implementation.
    private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var loginSession = (ILoginSession)sender;
        if (e.PropertyName == "State")
        {
            if (loginSession.State == LoginState.LoggedIn)
            {
                bool connectAudio = true;
                bool connectText = true;

                // This puts you into an echo channel where you can hear yourself speaking.
                // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
                JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
                // To test with multiple users, try joining a non-positional channel.
                // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
            }
        }
    }*/
}
