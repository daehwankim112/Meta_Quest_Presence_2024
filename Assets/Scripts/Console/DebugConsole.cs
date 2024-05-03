using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text debugConsole;

    private static event Action<string, LogType> OnDebugMessage;
    public static void Log(string message)
    {
        OnDebugMessage?.Invoke(message, LogType.Log);
    }

    public static void Error(string message)
    {
        OnDebugMessage?.Invoke(message, LogType.Error);
    }
    
    public static void Warn(string message)
    {
        OnDebugMessage?.Invoke(message, LogType.Warning);
    }

    public static void Success(string message)
    {
        OnDebugMessage?.Invoke(message, LogType.Assert);
    }

    void OnEnable()
    {
        OnDebugMessage += HandleDebugMessage;
    }

    void OnDisable()
    {
        OnDebugMessage -= HandleDebugMessage;
    }

    void HandleDebugMessage(string message, LogType logType)
    {
        Color color = logType switch
        {
            LogType.Error => Color.red,
            LogType.Warning => Color.yellow,
            LogType.Log => Color.white,
            LogType.Assert => Color.green,
            _ => Color.white
        };

        debugConsole.text += $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n";
    }
}