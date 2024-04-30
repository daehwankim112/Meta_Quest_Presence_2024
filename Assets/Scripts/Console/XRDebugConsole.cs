using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class XRDebugConsole : MonoBehaviour
{
    const string SHOW_LOGS_PREF = "xr_debug_show_logs";
    const string SHOW_WARNINGS_PREF = "xr_debug_show_warnings";
    const string SHOW_ERRORS_PREF = "xr_debug_show_errors";
    
    [SerializeField] VerticalLayoutGroup messagesLayout;
    [SerializeField] XRDebugMessageUI debugMessagePrefab;

    [SerializeField] Color logColor;
    [SerializeField] Color warningColor;
    [SerializeField] Color errorColor;

    [SerializeField] XRDebugLogTypeButton toggleLogsButton;
    [SerializeField] XRDebugLogTypeButton toggleWarningsButton;
    [SerializeField] XRDebugLogTypeButton toggleErrorsButton;

    bool _showLogs = true;
    bool _showWarnings = true;
    bool _showErrors = true;

    List<XRDebugMessageUI> _logs = new();
    static List<(string message, string stackTrace, LogType type)> preSceneLogs = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitializeInstance()
    {
        Application.logMessageReceived += HandlePreSceneLog;
    }

    void OnEnable()
    {
        foreach (var log in preSceneLogs)
        {
            HandleLog(log.message, log.stackTrace, log.type);
        }
        preSceneLogs.Clear();
        
        Application.logMessageReceived -= HandlePreSceneLog;
        Application.logMessageReceived += HandleLog;
        
        toggleLogsButton.AddListener(ToggleLogs);
        toggleWarningsButton.AddListener(ToggleWarnings);
        toggleErrorsButton.AddListener(ToggleErrors);
        
        if (PlayerPrefs.HasKey(SHOW_LOGS_PREF)) _showLogs = PlayerPrefs.GetInt(SHOW_LOGS_PREF) == 1;
        if (PlayerPrefs.HasKey(SHOW_WARNINGS_PREF)) _showWarnings = PlayerPrefs.GetInt(SHOW_WARNINGS_PREF) == 1;
        if (PlayerPrefs.HasKey(SHOW_ERRORS_PREF)) _showErrors = PlayerPrefs.GetInt(SHOW_ERRORS_PREF) == 1;

        RefreshLogs();
        RefreshButtons();
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        
        toggleLogsButton.RemoveAllListeners();
        toggleWarningsButton.RemoveAllListeners();
        toggleErrorsButton.RemoveAllListeners();
    }

    void ToggleLogs()
    {
        _showLogs = !_showLogs;
        SaveLogPrefs(LogType.Log, _showLogs);
        ToggleLogTypes(LogType.Log);
    }
    void ToggleWarnings()
    {
        _showWarnings = !_showWarnings;
        SaveLogPrefs(LogType.Warning, _showWarnings);
        ToggleLogTypes(LogType.Warning);
    }
    void ToggleErrors()
    {
        _showErrors = !_showErrors;
        SaveLogPrefs(LogType.Error, _showErrors);
        ToggleLogTypes(LogType.Error);
    }

    static void SaveLogPrefs(LogType type, bool value)
    {
        int intValue = value ? 1 : 0;
        string key = type switch
        {
            LogType.Log => SHOW_LOGS_PREF,
            LogType.Warning => SHOW_WARNINGS_PREF,
            LogType.Error => SHOW_ERRORS_PREF,
            LogType.Exception => SHOW_ERRORS_PREF,

            _ => string.Empty
        };
        
        PlayerPrefs.SetInt(key, intValue);
    }

    bool ShouldShowLog(LogType type)
    {
        return type switch
        {
            LogType.Log => _showLogs,
            LogType.Warning => _showWarnings,
            LogType.Error => _showErrors,
            LogType.Assert => _showErrors,

            _ => false
        };
    }

    void ToggleLogTypes(LogType logType)
    {
        foreach (var msg in _logs.Where(msg => msg.Type == logType))
        {
            bool toggle = ShouldShowLog(logType);
            msg.gameObject.SetActive(toggle);
        }
    }

    void RefreshLogs()
    {
        IEnumerable<XRDebugMessageUI> logs = _logs.Where(msg => msg.Type == LogType.Log);
        IEnumerable<XRDebugMessageUI> warnings = _logs.Where(msg => msg.Type == LogType.Warning);
        IEnumerable<XRDebugMessageUI> errors = _logs.Where(msg => msg.Type is LogType.Error or LogType.Exception);
        
        foreach (var msg in logs)
        {
            bool toggle = ShouldShowLog(msg.Type);
            msg.gameObject.SetActive(toggle);
        }
        foreach (var msg in warnings)
        {
            bool toggle = ShouldShowLog(msg.Type);
            msg.gameObject.SetActive(toggle);
        }
        foreach (var msg in errors)
        {
            bool toggle = ShouldShowLog(msg.Type);
            msg.gameObject.SetActive(toggle);
        }
    }

    void RefreshButtons()
    {
        toggleLogsButton.SetVisualState(_showLogs);
        toggleWarningsButton.SetVisualState(_showWarnings);
        toggleErrorsButton.SetVisualState(_showErrors);
    }
    
    void HandleLog(string message, string stackTrace, LogType type)
    {
        XRDebugMessageUI newMessage = Instantiate(debugMessagePrefab, messagesLayout.transform, false);
        newMessage.Initialize(message, stackTrace, type, GetLogColor(type));
        newMessage.gameObject.SetActive(ShouldShowLog(type));
        _logs.Add(newMessage);
    }

    static void HandlePreSceneLog(string message, string stackTrace, LogType type)
    {
        preSceneLogs.Add((message, stackTrace, type));
    }

    public Color GetLogColor(LogType type)
    {
        return type switch
        {
            LogType.Log => logColor,
            LogType.Warning => warningColor,
            LogType.Error => errorColor,
            _ => Color.white
        };
    }
}