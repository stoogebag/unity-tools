#if UNITASK
#if UNIRX
      

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class DelayedBoolWithProgress: IProgress<float>
{
    
    private Func<UniTask> onTrueFunc { get; set; }
    protected Func<UniTask> onFalseFunc { get; set; }

    private CancellationTokenSource _cts = new CancellationTokenSource();   
    
    
    private BoolReactiveProperty UnderlyingValue = new BoolReactiveProperty();
    public BoolReactiveProperty Value = new BoolReactiveProperty();
    
    public FloatReactiveProperty Progress { get; } = new FloatReactiveProperty();
    
    //public bool ResetProgressOnFalse = true; todo: hmm possible? 

    private DateTime _lastSetTrue;

    public TimeSpan Delay;
    
    public DelayedBoolWithProgress(){}
    
    
    //delayinseconds is the time taken before we register a change. designed to prevent flicker
    public DelayedBoolWithProgress(Func<UniTask> onTrueFunc, Func<UniTask> onFalseFunc, float delayInSeconds, bool initialValue)
    {
        //when you set underlyingvalue = true, it runs beforeTrueFunc,, then onTrueFunc
        
        
        this.onTrueFunc = onTrueFunc;
        this.onFalseFunc = onFalseFunc;

        UnderlyingValue = new BoolReactiveProperty(initialValue);
        Value = new BoolReactiveProperty(initialValue);
        Delay = TimeSpan.FromSeconds(delayInSeconds);
        
        
        UnderlyingValue.Throttle(Delay)?.Subscribe(async b=>
        {
            {
                _cts.Cancel();
                var myTask = b ? onTrueFunc() : onFalseFunc();
                
                await myTask.AttachExternalCancellation(_cts.Token);
                if(myTask.Status == UniTaskStatus.Succeeded)
                {
                    Value.Value = b;
                    Debug.Log("value is now " + b);
                }
                else
                {
                    Debug.Log("task cancelled. value not set.");
                }
            }
        });
    }
    
    public void SetValue(bool b)
    {
        //Debug.Log($"setvalue {b}");
        UnderlyingValue.Value = b;
    }


    // public enum State { True, False, TransitioningToTrue, TransitioningToFalse }
    //
    // public State CurrentState
    // {
    //     get
    //     {
    //         if (onTrue.IsPlaying()) return State.TransitioningToTrue;
    //         if (onFalse.IsPlaying()) return State.TransitioningToFalse;
    //         return Value.Value ? State.True : State.False;
    //     }
    // }
    public void Report(float value)
    {
        Debug.Log("progress is now " + value);
    }
}
#endif
#endif