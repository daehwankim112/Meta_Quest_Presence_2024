using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    string _joinCode = string.Empty;
    
    public void Initialize(string joinCode)
    {
        _joinCode = joinCode;
    }

    public void JoinLobby()
    {
        NetworkConnect.Join(_joinCode);
    }
}
