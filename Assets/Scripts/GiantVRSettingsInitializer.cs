using UnityEngine;

public class GiantVRSettingsInitializer : MonoBehaviour
{
    void OnEnable()
    {
        OVRManager.display.RecenteredPose += OnResetView;
    }

    void OnDisable()
    {
        OVRManager.display.RecenteredPose -= OnResetView;
    }

    void OnResetView()
    {
        Debug.LogError("Reset View");
    }
    
    void Start()
    {
        OVRManager.instance.AllowRecenter = false;
    }
}
