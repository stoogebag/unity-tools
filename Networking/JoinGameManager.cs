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
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class JoinGameManager : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
    }

    private void Awake()
    {
        var hostButton = gameObject.FirstOrDefault<Button>("ButtonHost");
        hostButton.OnClickAsObservable().Subscribe((s) =>
        {
            print("clicked host");
            var l = LoadingPopup.Open();
            l.SetMessage("starting server");
            StartServer();
            l.Close();
        });

    }

    private void StartServer()
    {
        if (_networkManager == null)
            return;

        _networkManager.ServerManager.StartConnection();
    }
}

#endif