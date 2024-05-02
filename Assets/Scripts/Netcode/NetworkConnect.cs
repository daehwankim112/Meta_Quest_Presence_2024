// David Kim 2024/4/27
// Description: Use Relay and Netcode to create a networked VR experience. This script is used to connect to the network and create or join a room. This script is used to connect to the network and create or join a room.
// Following the tutorial from https://youtu.be/Pry4grExYQQ?si=7Jh1pwQdKrPFnWrz and https://youtu.be/sPKS3vjwvpU?si=4zhDWuL8SApYPniC

using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

[HelpURL("https://youtu.be/Pry4grExYQQ?si=7Jh1pwQdKrPFnWrz")]

public class NetworkConnect : MonoBehaviour
{
    public static NetworkConnect instance;
    
    [SerializeField] TextMeshProUGUI debugConsole;

    [SerializeField] int maxConnections = 20;
    [SerializeField] UnityTransport transport;

    Lobby _currentLobbby;
    string _joinCode;

    async void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
        
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
            debugConsole.text += "Signed In" + AuthenticationService.Instance.PlayerId;
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Create()
    {
        try
        {
            Debug.Log("Host - Creating an allocation.");
            debugConsole.text += "Host - Creating an allocation.";

            // Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            // newJoinCode will be used to join the relay server
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // roomCodeTextMeshProUGUI.text = joinCode;
            Debug.Log("newJoinCode" + _joinCode);
            debugConsole.text += "newJoinCode" + _joinCode;
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            // Create a lobby
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>();
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, _joinCode);
            lobbyOptions.Data.Add("joinCode", dataObject);

            _currentLobbby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", maxConnections, lobbyOptions);
            
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            debugConsole.text += e.Message;
        }
    }

    public static async void Join(string relayJoinCode)
    {
        try
        {
            // if you are a host, you cannot join a relay
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("You are hosting! Cannot join a relay.");
                instance.debugConsole.text += "You are hosting! Cannot join a relay.";
                return;
            }

            Debug.Log("Joining Relay with " + relayJoinCode);
            instance.debugConsole.text += "Joining Relay with " + relayJoinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode); 
            instance.transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            
            NetworkManager.Singleton.StartClient();

            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("Client - Connected to the server.");
                instance.debugConsole.text += "Client - Connected to the server.";
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            instance.debugConsole.text += e.Message;
        }
    }

    public static string GetJoinCode(Lobby lobby)
    {
        return lobby.Data.TryGetValue("joinCode", out var joinCode) ? joinCode.Value : string.Empty;
    }
}
