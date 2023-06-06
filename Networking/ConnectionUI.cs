#if FISHNET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.FishyUnityTransport;
using ReactUnity.Helpers;
using RSG;
using UnityEngine;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using stoogebag.Utils;
using UniRx;
using UnityEngine.UI;

public class ConnectionUI : ReactUIManager
{
    private ReactiveProperty<int> myInt = new ReactiveProperty<int>();
    public ReactAction<int> OnIntChanged = new ReactAction<int>();

    protected override void Awake()
    {
        base.Awake();
        _networkManager = GetComponent<NetworkManager>();
    }

    private async void Start()
    {
        myInt.Subscribe(i => OnIntChanged?.Invoke(i));

        //await Task.Delay(1000);
        //var prom = HostClick();



    }

    private void FixedUpdate()
    {
        if (prommy != null) print(prommy.CurState);
    }

    public Promise prommy;


    private static event Action Done;

    public async Task HostClick()
    {
        //
        await NetworkConnectManager.Instance.StartServer(false);
        await  NetworkConnectManager.Instance.TryConnectAsHost();

        HostClickFinished?.Invoke();
    }
    public async Task HostClickOnline()
    {
        //
        await NetworkConnectManager.Instance.StartServer(true);
        await  NetworkConnectManager.Instance.TryConnectAsHost();

        HostClickOnlineFinished?.Invoke();
    }
    
    

    public ReactAction HostClickFinished = new ReactAction();
    public ReactAction HostClickOnlineFinished = new ReactAction();

    public  IPromise<string> OnHostClick()
    {

        return new Promise<string>(async (res, err) =>
        {

            print(1);
            await Task.Delay(1000);

            print(2);
            res("butt!");
        });


    }
    public  IPromise OnHostClick2()
    {

        return new Promise(async (res, err) =>
        {

            print(1);
            await Task.Delay(1000);

            print(2);
            res();
        });


    }


    public async Task JoinClickLAN()
    {
        NetworkConnectManager.Instance.TryConnectLAN();
        JoinClickFinished?.Invoke();
    }
    public async Task JoinClickOnline()
    {
        NetworkConnectManager.Instance.TryConnectOnline();
        JoinClickFinished?.Invoke();
    }
    public ReactAction JoinClickFinished = new ReactAction();
    private NetworkManager _networkManager;
}

#endif