
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using stoogebag;
using UniRx;

public class Window : MonoBehaviour
{
    private IWindowAnimation _anim;

    public event Action OnActivated;
    public event Action OnDeactivated;

    protected CompositeDisposable _disposable = new();
    
    [SerializeField]
    private bool ActivateOnStart = false;

    protected virtual void Start()
    {

        if (ActivateOnStart) Activate();
    }


    public IWindowAnimation Anim {
        get
        {
            if (_anim == null) _anim = GetComponent<IWindowAnimation>();
            return _anim;
        }
    }
    
    public virtual async Task Activate()
    {
        if (Active) return;
        await Anim.Activate();
        Active = true;
        OnActivated?.Invoke();
    }
    public virtual async Task Deactivate()
    {
        if (!Active) return;
        await Anim.Deactivate();
        Active = false;
        OnDeactivated?.Invoke();
    }

    public bool Active = false;
}

public abstract class TemporaryWindow<TInputModel, TDataModel> : Window where TDataModel : class
{
    private CompositeDisposable _popupDisposable = new();
    public async Task<WindowResult> PopupAndAwaitResult(TInputModel inputs, TDataModel data = null)
    {
        Bind(inputs, data);
        await this.Activate();
        var close = new TaskCompletionSource<WindowResult>();

        ProceedObservable.Subscribe(m =>
        {
            close.TrySetResult(new WindowResult()
            {
                Result = Result.Proceed,
                Data = m
            });
        }).AddTo(_popupDisposable);

        CancelObservable.Subscribe(m =>
        {
            close.TrySetResult(new WindowResult()
            {
                Result = Result.Cancel,
            });
        }).AddTo(_popupDisposable);

        var result = await close.Task;
        _popupDisposable.Clear();
        await this.Deactivate(); //possibly dont bother awaiting this...
        return result;
    }


    public event Action<TDataModel> Proceed;
    public IObservable<TDataModel> ProceedObservable => Observable.FromEvent<TDataModel>(h => Proceed += h, h => Proceed -= h);
    public event Action Cancel;
    public IObservable<Unit> CancelObservable => Observable.FromEvent(h => Cancel += h, h => Cancel -= h);

    public void TryProceed()
    {
        if(VerifyProceed()) Proceed?.Invoke(GetModel());
    }
    public void TryCancel()
    {
        if (VerifyCancel()) Cancel?.Invoke();
    }
    
    
    protected virtual bool VerifyProceed()
    {
        return true;
    }

    protected virtual bool VerifyCancel()
    {
        return true;
    }
    
    protected abstract TDataModel GetModel();

    protected abstract void Bind(TInputModel input, TDataModel model = null);
    
    public enum Result
    {
        Proceed,
        Cancel,
        //ForceClose,
    }

    public class WindowResult
    {
        public TDataModel Data;
        public Result Result;
    }
    
}