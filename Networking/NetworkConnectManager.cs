#if FISHNET

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;

using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Yak;


public class NetworkConnectManager : PersistentSingleton<NetworkConnectManager>
{
    /// <summary>
    /// Found NetworkManager.
    /// </summary>
    private NetworkManager _networkManager;
    /// <summary>
    /// Current state of client socket.
    /// </summary>
    private LocalConnectionState _clientState = LocalConnectionState.Stopped;
    /// <summary>
    /// Current state of server socket.
    /// </summary>
    private LocalConnectionState _serverState = LocalConnectionState.Stopped;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _networkManager = InstanceFinder.NetworkManager;
    }

    public void StartServer()
    {
        _networkManager.ServerManager.StartConnection();
        _networkManager.GetComponent<Multipass>().SetClientTransport<Yak>();
        print(NetworkingExtensions.GetLocalIPAddress());
    }

    public void TryConnect()
    {

        _networkManager.ClientManager.StartConnection();
        
    }

    public void Disconnect()
    {
        
    }
    
}
#endif