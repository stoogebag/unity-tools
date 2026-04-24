#if FISHNET
using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools;
using stoogebag.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class JoinGameManager : Singleton<JoinGameManager>
{
    private PlayerInputManager _playerInputManager;
    private bool _focused;

    public bool AllowJoining;

    private void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        Application.focusChanged += OnApplicationFocusChanged;
        
        RefreshJoinStatus();
    }

    private void OnDestroy()
    {
        Application.focusChanged -= OnApplicationFocusChanged;
    }

    private void OnApplicationFocusChanged(bool val)
    {
        _focused = val;
        RefreshJoinStatus();
    }

    private void RefreshJoinStatus()
    {
        if(_focused && AllowJoining) _playerInputManager.EnableJoining();
        else _playerInputManager.DisableJoining();
    }
}

#endif