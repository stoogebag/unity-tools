using System;
using System.Threading.Tasks;
using stoogebag.Input;
using UnityEngine;
#if FISHNET
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using stoogebag;
using stoogebag.Utils;
using UniRx;

public abstract class MatchSetupManager<TMatchInfo> : Singleton<MatchSetupManager<TMatchInfo>>
    where TMatchInfo : MatchInfoBase
{

    public TMatchInfo MatchInfo;

    private void Awake()
    {
        base.Awake();

        //this.ObserveEveryValueChanged(t => t.Players).Subscribe(t => print(Players.Select(t => t.Name).ToList()));
    }

    private async void Start()
    {
        var client = NetworkClient.Instance;
        if (client == null)
        {
            isMockMatch = true;

            if (InstanceFinder.IsOffline)
            {
                await NetworkConnectManager.Instance.StartDebug();
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


/// <summary>
/// matchinfo has to be a networkbehaviour so that it can be synchronised. this sucks but that's life.
/// this is also why it can't be generic with a TMatchInfo or anything.
/// </summary>
public abstract class MatchInfoBase : NetworkBehaviour
{
    public int ID { get; private set; }

    public abstract void InitialiseDefaults();

    public abstract MatchInfoModel GetModel();

    
}

public abstract class MatchInfoModel
{
    public int ID;
}

#endif

namespace stoogebag.Networking.MatchSetup
{
    public class PlayerInfo
    {
        public string Name = "player";
        public string ID;
        public Color Color = Color.blue; //unsure should we do this.
        public bool Ready;
    
        [NonSerialized]
        public InputSchemeBase Input; //this may be lost across network operations, so we keep the controller guid in case we need to get it back at some point...
    

        public int connectionID;
        public string inputID = "";

        public void ComputeID()
        {
            inputID = (Input?.GetID() ?? "NONE");
            ID = connectionID + "-" + inputID;
        }


    }
}