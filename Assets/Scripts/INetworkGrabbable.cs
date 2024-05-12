using UnityEngine;

public interface INetworkGrabbable
{
    void Grabbed();
    public void Grabbing(Vector3 position);
    void Released(Vector3 direction);
}
