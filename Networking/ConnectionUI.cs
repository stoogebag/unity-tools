#if FISHNET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting;
using RSG;
using UnityEngine;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using stoogebag.Utils;
using UniRx;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    //public ReactAction<int> OnIntChanged = new ReactAction<int>();

    protected  void Awake()
    {
        //base.Awake();
        _networkManager = GetComponent<NetworkManager>();
    }

    private async void Start()
    {
        // myInt.Subscribe(i => OnIntChanged?.Invoke(i));

        //await Task.Delay(1000);
        //var prom = HostClick();



    }

    private void FixedUpdate()
    {
        if (prommy != null) print(prommy.CurState);
    }

    public Promise prommy;


    private static event Action Done;

    public async Task HostClickLocal()
    {
        print("host local...");
        //
        await NetworkConnectManager.Instance.StartServer(false);
        await  NetworkConnectManager.Instance.TryConnectAsHost();

        // HostClickFinished?.Invoke();
    }
    public async Task HostClickOnline()
    {
        
        print("host online...");
        //
        await NetworkConnectManager.Instance.StartServer(true);
        await  NetworkConnectManager.Instance.TryConnectAsHost();

        // HostClickOnlineFinished?.Invoke();
    }
    
    public async Task JoinClickLocal()
    {
        await NetworkConnectManager.Instance.TryConnectLAN();
        // JoinClickFinished?.Invoke();
    }
    public async Task JoinClickOnline()
    {
        await NetworkConnectManager.Instance.TryConnectOnline();
        // JoinClickFinished?.Invoke();
    }
    // public ReactAction JoinClickFinished = new ReactAction();
    private NetworkManager _networkManager;
}

#endif