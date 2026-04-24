using System;
using AxonGenesis;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TimeflowObservableProvider : MonoBehaviour, ITimeflowPlayback
{
    Subject<Unit> onPlay = new Subject<Unit>();
    public IObservable<Unit> OnPlayObservable => onPlay.AsObservable();

    Subject<Unit> onLoop = new Subject<Unit>();
    public IObservable<Unit> OnLoopObservable => onLoop.AsObservable();

    Subject<Unit> onStop = new Subject<Unit>();
    public IObservable<Unit> OnStopObservable => onStop.AsObservable();

    Subject<Unit> onRewind = new Subject<Unit>();
    public IObservable<Unit> OnRewindObservable => onRewind.AsObservable();

    Subject<Unit> onUpdate = new Subject<Unit>();
    public IObservable<Unit> OnUpdateObservable => onUpdate.AsObservable();

    public Timeflow TimeflowParent { get; set; }

    private void Awake()
    {
        TimeflowParent = gameObject.GetComponent<Timeflow>();
        TimeflowParent.RegisterPlaybackListener(this);
        
        this.OnDestroyAsObservable()
            .Subscribe(_ => TimeflowParent.UnregisterPlaybackListener(this))
            .AddTo(this);
    }

    public void OnPlay()
    {
        onPlay.OnNext(Unit.Default);
    }

    public void OnLoop()
    {
        onLoop.OnNext(Unit.Default);
    }

    public void OnStop()
    {
        onStop.OnNext(Unit.Default);
    }

    public void OnRewind()
    {
        onRewind.OnNext(Unit.Default);
    }

    public void OnUpdate()
    {
        onUpdate.OnNext(Unit.Default);
    }
}