using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkConnect : MonoBehaviour
{
    private string joinCode;
    public TMPro.TMP_InputField joinCodeInputTextMeshPro;
    public TMPro.TextMeshProUGUI roomCodeTextMeshProUGUI;

    public int maxConnections = 20;
    public UnityTransport transport;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Create()
    {
        try
        {
            Debug.Log("Host - Creating an allocation.");

            // Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            // newJoinCode will be used to join the relay server
            string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            roomCodeTextMeshProUGUI.text = newJoinCode;
            Debug.Log("newJoinCode" + newJoinCode);
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }

    public async void Join()
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode); 
            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void ClientInput()
    {
        joinCode = joinCodeInputTextMeshPro.text;
    }
}
