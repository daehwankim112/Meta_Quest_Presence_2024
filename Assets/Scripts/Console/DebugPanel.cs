using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text debugConsole;

    private static event Action<string> OnDebugMessage;
    public static void Send(string message)
    {
        OnDebugMessage?.Invoke(message);
    }

    void OnEnable()
    {
        OnDebugMessage += HandleDebugMessage;
    }

    void OnDisable()
    {
        OnDebugMessage -= HandleDebugMessage;
    }

    void HandleDebugMessage(string message)
    {
        debugConsole.text += message + "\n";
    }
}
