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

[HelpURL("https://youtu.be/Pry4grExYQQ?si=7Jh1pwQdKrPFnWrz")]

public class NetworkConnect : MonoBehaviour
{
    private string joinCode;
    public TMPro.TMP_InputField joinCodeInputTextMeshPro;
    public TMPro.TextMeshProUGUI roomCodeTextMeshProUGUI;
    public TMPro.TextMeshProUGUI DebugConsole;

    public int maxConnections = 20;
    public UnityTransport transport;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
            DebugConsole.text += "Signed In" + AuthenticationService.Instance.PlayerId;
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Create()
    {
        try
        {
            Debug.Log("Host - Creating an allocation.");
            DebugConsole.text += "Host - Creating an allocation.";

            // Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            // newJoinCode will be used to join the relay server
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            roomCodeTextMeshProUGUI.text = joinCode;
            Debug.Log("newJoinCode" + joinCode);
            DebugConsole.text += "newJoinCode" + joinCode;
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            DebugConsole.text += e.Message;
        }
    }

    public async void Join()
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCodeInputTextMeshPro.text);
            DebugConsole.text += "Joining Relay with " + joinCodeInputTextMeshPro.text;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInputTextMeshPro.text); 
            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();

            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("Client - Connected to the server.");
                DebugConsole.text += "Client - Connected to the server.";
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            DebugConsole.text += e.Message;
        }
    }

    public void ClientInput()
    {
        // joinCode = joinCodeInputTextMeshPro.text;
    }
}
