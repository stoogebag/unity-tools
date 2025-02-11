#if UNITASK
#if DOTWEEN
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


//delayedbool lets you set a bool, but it only registers the change in value as reactiveProperty Value after a tween has finished.
//todo: make it a unitask?
//todo: make it extend boolReactiveProperty instead of having its own Value prop?
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
    bool _reverseProgressOnFalse;
    
    
    public FloatReactiveProperty Progress { get; } = new FloatReactiveProperty();
    
    public DelayedBool(){}
    
    public DelayedBool(Func<Tween> onTrueFunc, Func<Tween> onFalseFunc, float delayInSeconds, bool initialValue, bool reverseProgressOnFalse = true)
    {
        this.onTrueFunc = onTrueFunc;
        this.onFalseFunc = onFalseFunc;
        Delay = TimeSpan.FromSeconds(delayInSeconds);

        UnderlyingValue = new BoolReactiveProperty(initialValue);
        Value = new BoolReactiveProperty(initialValue);
        
        UnderlyingValue?.Throttle(Delay).Subscribe(b=>StartTween(b));
        _reverseProgressOnFalse = reverseProgressOnFalse;
        // owner?.UpdateAsObservable().Subscribe(_ =>
        // {
        //     if (_progressFunc != null)
        //     {
        //         Progress.Value = _progressFunc.Invoke();
        //     }
        // }).DisposeWith(owner);

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
            
            //_progressFunc = ()=>onTrue.ElapsedPercentage();
            
            
            //Debug.Log($"onTrue Start");
            onTrue = onTrueFunc.Invoke();
            onTrue.OnUpdate(() => Progress.Value = onTrue.ElapsedPercentage());
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
            
            onFalse.OnUpdate(() => Progress.Value = 0);
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

#endif
#endif