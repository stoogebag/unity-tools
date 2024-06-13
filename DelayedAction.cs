using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class DelayedAction
{
    
    public Tween onTry;
    public Tween onCancel;

    //private BoolReactiveProperty UnderlyingValue = new BoolReactiveProperty();
    public event Action OnActivate;
    private BoolReactiveProperty UnderlyingValue = new BoolReactiveProperty();
    

    private DateTime _lastSetTrue;

    public TimeSpan Delay = TimeSpan.FromSeconds(0.1f);
    
    public DelayedAction()
    {
        UnderlyingValue?.Throttle(Delay).Subscribe(b=>StartTween(b));
    }
    
    public void SetValue(bool b)
    {
        //Debug.Log($"setvalue {b}");
        UnderlyingValue.Value = b;
    }

    private void StartTween(bool b) 
    {
       // Debug.Log($"startTween {b}");
        if (b)
        {
            onCancel?.Pause(); 
            if (onTry?.IsPlaying() == true) return;
            
            //Debug.Log($"onTrue Start");
            onTry.Restart();
            
            onTry.OnComplete(() =>
            {
                //Debug.Log($"onTrue Complete");
                OnActivate?.Invoke();
                SetValue(false);
            });
        }
        else
        {
            onTry?.Pause(); 
            if (onCancel?.IsPlaying() == true) return;
            
            //Debug.Log($"onFalse Start");
            onCancel.Restart();
            
            onCancel.OnComplete(() =>
            {
            });
        }
    }

    //public enum State { True, False, TransitioningToTrue, TransitioningToFalse }
    
    // public State CurrentState
    // {
    //     get
    //     {
    //         if (onTry.IsPlaying()) return State.TransitioningToTrue;
    //         if (onCancel.IsPlaying()) return State.TransitioningToFalse;
    //         return Value.Value ? State.True : State.False;
    //     }
    // }
}