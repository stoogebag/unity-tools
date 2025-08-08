#if UNITASK
#if UNIRX

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MoveAndAwaitBehaviour : PlayableBehaviour
{
    public TweenToPos targetPos;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if (!started)
        {
            started = true;
            task = targetPos.MoveToPosition(playerData as Transform);
            task.ToObservable().Subscribe(unit => playable.GetDirector().Resume()).AddTo(_disposable);
            playable.GetDirector().Pause();
        }


    }


    private UniTask task;
    private bool started = false;
    private CompositeDisposable _disposable = new();
}

#endif
#endif