#if FISHNET
using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Transporting;
using UnityEngine;
using stoogebag;

public class NetworkClient : NetworkSingleton<NetworkClient>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public Transport transport;

    //networkclient holds the initial match info sends it to matchmanager in case it's needed 
    public MatchInfoBase InitialMatchInfo { get; set; }
}

#endif