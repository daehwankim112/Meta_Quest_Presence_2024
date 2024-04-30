using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class XRDebugLogTypeButton : MonoBehaviour
{
    [SerializeField] Image backer;
    [SerializeField] Image icon;
    [SerializeField] Button button;

    bool _enabled;

    public void AddListener(UnityAction action) => button.onClick.AddListener(ToggleVisualState + action);
    public void RemoveAllListeners() => button.onClick.RemoveAllListeners();

    void ToggleVisualState()
    {
        SetVisualState(!_enabled);
    }
    
    public void SetVisualState(bool enabled)
    {
        _enabled = enabled;
        backer.color = enabled ? Color.white : new Color(backer.color.r, backer.color.g, backer.color.b, 0.5f);
        icon.color = enabled ? Color.white : new Color(0.25f, 0.25f, 0.25f, 1f);
    }
}