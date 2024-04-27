using TMPro;
using UnityEngine;

public class XRDebugMessageUI : MonoBehaviour
{
    public LogType Type { get; private set; }
    
    [SerializeField] TMP_Text messageText;
    [SerializeField] TMP_Text stackTraceText;

    public void Initialize(string message, string stackTrace, LogType type, Color messageColor)
    {
        Type = type;
        string color = ColorUtility.ToHtmlStringRGB(messageColor);
        messageText.text = $"<color=#{color}>{message}</color>";
        stackTraceText.text = $"<color=#{color}>{stackTrace}</color>";
    }
}