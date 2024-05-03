using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] TMP_Text textBox;

    static event Action<object, LogType> OnDebugMessage;
    public static void Log(object message) => OnDebugMessage?.Invoke(message, LogType.Log);
    public static void Error(object message) => OnDebugMessage?.Invoke(message, LogType.Error);
    public static void Warn(object message) => OnDebugMessage?.Invoke(message, LogType.Warning);
    public static void Success(object message) => OnDebugMessage?.Invoke(message, LogType.Assert);

    void OnEnable() => OnDebugMessage += HandleDebugMessage;
    void OnDisable() => OnDebugMessage -= HandleDebugMessage;

    void HandleDebugMessage(object message, LogType logType)
    {
        if (textBox == null) return;
        
        Color color = logType switch
        {
            LogType.Error => Color.red,
            LogType.Warning => Color.yellow,
            LogType.Log => Color.white,
            LogType.Assert => Color.green,
            _ => Color.white
        };

        textBox.text += $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n";
    }
}