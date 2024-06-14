using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class DelayedBool
{
    private Tween onTrue;
    private Tween onFalse;
    
    private Func<Tween> onTrueFunc { get; set; }
    protected Func<Tween> onFalseFunc { get; set; }
    

    private BoolReactiveProperty UnderlyingValue = new BoolReactiveProperty();
    public BoolReactiveProperty Value = new BoolReactiveProperty();

    private DateTime _lastSetTrue;

    public TimeSpan Delay;
    
    public DelayedBool(){}
    
    public DelayedBool(Func<Tween> onTrueFunc, Func<Tween> onFalseFunc, float delayInSeconds, bool initialValue)
    {
        this.onTrueFunc = onTrueFunc;
        this.onFalseFunc = onFalseFunc;
        Delay = TimeSpan.FromSeconds(delayInSeconds);

        UnderlyingValue = new BoolReactiveProperty(initialValue);
        Value = new BoolReactiveProperty(initialValue);
        
        UnderlyingValue?.Throttle(Delay).Subscribe(b=>StartTween(b));
    }
    
    public void SetValue(bool b)
    {
        //Debug.Log($"setvalue {b}");
        UnderlyingValue.Value = b;
    }

    public async UniTask SetValueAwaitable(bool b) //ONLY USE IF YOU KNOW IT WONT CANCEL EVER
    {
        if (Value.Value == b) return;
        
        UnderlyingValue.Value = b;
        await UniTask.WaitUntil(() => Value.Value == b);
    }
    

    private void StartTween(bool b) 
    {
        if (b)
        {
            onFalse?.Pause().Kill(); 
            if (onTrue?.IsPlaying() == true) return;
            
            //Debug.Log($"onTrue Start");
            onTrue = onTrueFunc.Invoke();
            onTrue.Restart();
            
            onTrue.OnComplete(() =>
            {
                //Debug.Log($"onTrue Complete");
                Value.Value = true;
            });
        }
        else
        {
            onTrue?.Pause().Kill(); 
            if (onFalse?.IsPlaying() == true) return;
            
            //Debug.Log($"onFalse Start");

            onFalse = onFalseFunc.Invoke();
            onFalse.Restart();
            
            onFalse.OnComplete(() =>
            {
                //Debug.Log($"onFalse Complete");
                Value.Value = false;
            });
        }
    }

    public enum State { True, False, TransitioningToTrue, TransitioningToFalse }
    
    public State CurrentState
    {
        get
        {
            if (onTrue.IsPlaying()) return State.TransitioningToTrue;
            if (onFalse.IsPlaying()) return State.TransitioningToFalse;
            return Value.Value ? State.True : State.False;
        }
    }
}