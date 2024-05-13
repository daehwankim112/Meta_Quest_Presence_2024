using System.Collections.Generic;
using NuiN.NExtensions;
using Unity.Netcode;
using UnityEngine;

public class WaterDeathController : NetworkBehaviour
{
    public static float WaterHeight { get; private set; }
    
    [SerializeField] float waterHeight;
    
    [SerializeField] float boatMinHeight = 100f;
    [SerializeField] float boatMaxHeight = 125f;
    
    [SerializeField] float spawnRadius = 100f;
    [SerializeField] RespawnBoat boatPrefab;
    
    List<RespawnBoat> _activeBoats = new ();

    void Awake()
    {
        WaterHeight = waterHeight;
    }

    void OnEnable()
    {
        GameEvents.OnSmallPlayerFellInWater += SpawnBoatRespawnPlayerServerRpc;
    }
    void OnDisable()
    {
        GameEvents.OnSmallPlayerFellInWater -= SpawnBoatRespawnPlayerServerRpc;
    }
    
    void Update()
    {
        DespawnCompletedBoats();
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnBoatRespawnPlayerServerRpc(ulong playerID)
    {
        Vector2 randomRadius = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 boatSpawnPos = new Vector3(randomRadius.x, Random.Range(boatMinHeight, boatMaxHeight), randomRadius.y);
        RespawnBoat boat = Instantiate(boatPrefab, boatSpawnPos, Quaternion.identity);
        boat.NetworkObject.Spawn();
        boat.destination = Vector3.zero.With(y: Random.Range(boatMinHeight, boatMaxHeight));
        Vector3 dirToDestination = VectorUtils.Direction(boatSpawnPos, boat.destination);
        boat.destination += dirToDestination.With(y: 0) * Vector3.Distance(boatSpawnPos.With(y: 0), boat.destination.With(y: 0));
        
        SetClientPosition(boat.PlayerSpawnPos, playerID);
        
        _activeBoats.Add(boat);
    }

    void SetClientPosition(Vector3 position, ulong playerID)
    {
        SetClientPositionClientRpc(position, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {playerID}}});
    }

    [ClientRpc]
    void SetClientPositionClientRpc(Vector3 position, ClientRpcParams clientRpcParams)
    {
        GameEvents.InvokeSetPlayerPosition(position);
    }

    void DespawnCompletedBoats()
    {
        if (!IsServer) return;
        
        for (int i = _activeBoats.Count-1; i >= 0; i--)
        {
            RespawnBoat boat = _activeBoats[i];

            if (boat == null)
            {
                _activeBoats.Remove(boat);
                continue;
            }
            if (boat.ReachedDestination())
            {
                _activeBoats.Remove(boat);
                boat.NetworkObject.Despawn();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(0, waterHeight, 0), new Vector3(100, 0, 100));
        Gizmos.color = Color.white;
    }
}