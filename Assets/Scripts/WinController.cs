using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WinController : NetworkBehaviour
{
    [SerializeField] int numRepairsNeeded;
    public NetworkVariable<int> numRepairs = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        numRepairs.Initialize(this);
        GameEvents.OnCannonRepaired += RepairedCannon;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameEvents.OnCannonRepaired -= RepairedCannon;
    }

    void RepairedCannon()
    {
        Debug.LogError("Repaired Cannon");
        
        numRepairs.Value++;
        if (numRepairs.Value >= numRepairsNeeded)
        {
            SmallPlayersWin();
        }
    }

    void SmallPlayersWin()
    {
        Debug.LogError("Small players won!");
    }
}