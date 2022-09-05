#if FISHNET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using stoogebag;
using UniRx;

public abstract class MatchSetupManager<TMatchInfo> : Singleton<MatchSetupManager<TMatchInfo>>
    where TMatchInfo : MatchInfoBase
{
    public SynchronisedMatchInfo MatchInfoSync;

    public TMatchInfo MatchInfo;

    private void Awake()
    {
        base.Awake();

        MatchInfoSync = GetComponent<SynchronisedMatchInfo>();
        //this.ObserveEveryValueChanged(t => t.Players).Subscribe(t => print(Players.Select(t => t.Name).ToList()));
    }

    private void Start()
    {
        var client = NetworkClient.Instance;
        if (client == null)
        {
            isMockMatch = true;

            if (InstanceFinder.IsOffline)
            {
                NetworkConnectManager.Instance.StartServer();
                NetworkConnectManager.Instance.TryConnect();
            }

            //var info = GetMatchInfo();
            Bind(GetMatchInfo());
        }
    }

    protected abstract void Bind(TMatchInfo matchInfo);
    //protected abstract void InitialiseMockMatch(TMatchInfo matchInfo);
    //protected abstract MatchManager<TMatchInfo> CreateMatchManager();

    //protected abstract TMatchInfo GetDefaultMatchInfo();
    protected abstract TMatchInfo GetMatchInfo();

    public event Action<TMatchInfo> MatchInfoChanged;
    public IObservable<TMatchInfo> MatchInfoChangedObservable => Observable.FromEvent<TMatchInfo>(h => MatchInfoChanged += h, h => MatchInfoChanged -= h);

    protected void RaiseMatchInfoChanged(TMatchInfo info)
    {
        MatchInfoChanged?.Invoke(info);
    }



    private bool isMockMatch = false;

    public abstract void StartMatch();
}

public class PlayerInfo
{
    private static int count;
    public string Name = "player";
    //public bool isLocal;
    public int connectionID;
    
    [NonSerialized]
    public InputSchemeBase Input; //this may be lost across network operations, so we keep the controller guid in case we need to get it back at some point...
    
    public Color Color = Color.blue; //unsure should we do this.

    public string ID;
    public string inputID = "";

    public void ComputeID()
    {
        inputID = (Input?.GetID() ?? "NONE");
        ID = connectionID + "-" + inputID;
    }


}

/// <summary>
/// matchinfo has to be a networkbehaviour so that it can be synchronised. this sucks but that's life.
/// </summary>
public abstract class MatchInfoBase : NetworkBehaviour
{
    public int ID { get; private set; }

    public abstract void InitialiseDefaults();
}
#endif