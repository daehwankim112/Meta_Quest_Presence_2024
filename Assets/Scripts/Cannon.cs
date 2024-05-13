using NuiN.NExtensions;
using SpleenTween;
using Unity.Netcode;
using UnityEngine;

public class Cannon : NetworkBehaviour
{
    public NetworkVariable<int> numResources = new();
    [SerializeField] int numResourcesNeeded;

    [SerializeField] GameObject damagedVisual;
    [SerializeField] GameObject repairedVisual;

    Vector3 _startScale;

    [SerializeField] float scaleUpDuration;
    [SerializeField] float scaleDownDuration;
    [SerializeField] Ease scaleUpEase;
    [SerializeField] Ease scaleDownEase;

    
    bool Repaired => numResources.Value > numResourcesNeeded;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        numResources.Initialize(this);
        _startScale = transform.localScale;
        damagedVisual.SetActive(true);
        repairedVisual.SetActive(false);
        numResources.OnValueChanged += TryRepair;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        numResources.OnValueChanged -= TryRepair;
    }

    void TryRepair(int oldAmount, int newAmount)
    {
        if (Repaired) return;
        
        Spleen.Scale(transform, _startScale, _startScale * 1.25f, scaleUpDuration).SetEase(scaleUpEase).OnComplete(() =>
        {
            Spleen.Scale(transform, transform.localScale, _startScale, scaleDownDuration).SetEase(scaleDownEase);
        });
        
        Debug.LogError("Added Resource to Cannon, remaining: " + (numResourcesNeeded - numResources.Value));
            
        if (newAmount < numResourcesNeeded) return;
        
        damagedVisual.SetActive(false);
        repairedVisual.SetActive(true);
        
        GameEvents.InvokeCannonRepaired();
    }

    [ContextMenu("Add Resource")]
    public void AddResource()
    {
        if (Repaired) return;
        AddResourceServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void AddResourceServerRpc()
    {
        numResources.Value++;
    }
}