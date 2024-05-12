using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class WaterDeathController : MonoBehaviour
{
    public static float WaterHeight { get; private set; }
    
    [SerializeField] float waterHeight;
    
    [SerializeField] float boatMinHeight = 100f;
    [SerializeField] float boatMaxHeight = 125f;
    
    [SerializeField] float spawnRadius = 100f;
    [SerializeField] RespawnBoat boatPrefab;
    
    List<(RespawnBoat boat, SmallPlayer player)> _activeBoats = new ();

    void Awake()
    {
        WaterHeight = waterHeight;
    }

    void OnEnable()
    {
        GameEvents.OnSmallPlayerFellInWater += StartRespawnPlayer;
    }
    void OnDisable()
    {
        GameEvents.OnSmallPlayerFellInWater -= StartRespawnPlayer;
    }
    
    void Update()
    {
        MoveBoats();
    }

    void StartRespawnPlayer(SmallPlayer player)
    {
        Vector2 randomRadius = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 boatSpawnPos = new Vector3(randomRadius.x, Random.Range(boatMinHeight, boatMaxHeight), randomRadius.y);
        RespawnBoat boat = Instantiate(boatPrefab, boatSpawnPos, Quaternion.identity);
        boat.SetDestination(Vector3.zero.With(y: Random.Range(boatMinHeight, boatMaxHeight)));
        player.transform.position = boat.PlayerSpawnPos;
        
        _activeBoats.Add((boat, player));
    }

    void MoveBoats()
    {
        for (int i = _activeBoats.Count-1; i >= 0; i--)
        {
            (RespawnBoat boat, SmallPlayer player) boatWithPlayer = _activeBoats[i];

            if (boatWithPlayer.boat.ReachedDestination())
            {
                _activeBoats.Remove(boatWithPlayer);
                Destroy(boatWithPlayer.boat.gameObject);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(0, waterHeight, 0), new Vector3(100, 0, 100));
        Gizmos.color = Color.white;
    }
}