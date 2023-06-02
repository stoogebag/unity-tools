#if FISHNET

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using stoogebag;

using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.FishyUnityTransport;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using FishNet.Transporting.Yak;
using stoogebag.Utils;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;


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

    public async Task StartServer()
    {
        await StartRelayHost();
        //_networkManager.ServerManager.StartConnection();
        _networkManager.GetComponent<Multipass>().SetClientTransport<Yak>();
    }

    public async Task TryConnectAsHost()
    {
        _networkManager.GetComponent<Multipass>().SetClientTransport<Yak>();
        _networkManager.ClientManager.StartConnection();
    }
    public async Task TryConnectLAN()
    {
        _networkManager.GetComponent<Multipass>().SetClientTransport<Tugboat>();
        _networkManager.ClientManager.StartConnection();
    }
    public async Task TryConnectOnline()
    {
        await InitialiseUnityServices();
        
        _networkManager.GetComponent<Multipass>().SetClientTransport<FishyUnityTransport>();

        var serverData = await JoinRelayServerFromJoinCode(_joinCode);
        
        // var clientRelayUtilityTask = JoinRelayServerFromJoinCode(_joinCode);
        // while (!clientRelayUtilityTask.IsCompleted)
        // {
        //     
        // }
        // if (clientRelayUtilityTask.IsFaulted)
        // {
        //     Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
        //     return;
        // }
        
        
        var fut = _networkManager.GetComponent<Multipass>().GetTransport<FishyUnityTransport>();
        //fut.SetRelayServerData(clientRelayUtilityTask.Result);
        fut.SetRelayServerData(serverData);
        
        _networkManager.ClientManager.StartConnection();
    }

    private async Task InitialiseUnityServices()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsAuthorized)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerID = AuthenticationService.Instance.PlayerId;
        }
    }

    public void Disconnect()
    {
        
    }

    public MatchInfoModel MatchInfo;
    
    
    public async Task StartRelayHost()
    {
        _networkManager.GetComponent<Multipass>().SetClientTransport<FishyUnityTransport>();
        var utp = _networkManager.GetComponent<Multipass>().GetTransport<FishyUnityTransport>();

        // Setup HostAllocation
        try
        {
            await InitialiseUnityServices();
            
            
            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(4);
            utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));

            // Start Server Connection
            _networkManager.ServerManager.StartConnection();

            // Setup JoinAllocation
            // Remarks: It will currently work, but with a nasty 
            // bug (https://github.com/ooonush/FishyUnityTransport/issues/4).
            // This will be reworked in a future version.
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(_joinCode);
            utp.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        }
        catch (Exception e)
        {
            
        }
        // Start Client Connection
        //_networkManager.ClientManager.StartConnection();
    }

    [SerializeField] private string _joinCode;
    [SerializeField] private string _playerID;


    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError("Relay join request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        return new RelayServerData(allocation, "dtls");
    }


    public async Task StartDebug()
    {
         var fut = _networkManager.GetComponent<Multipass>().GetTransport<FishyUnityTransport>();
         DestroyImmediate(fut);
        
        
        await StartServer();
        await TryConnectAsHost();
    }
}
#endif