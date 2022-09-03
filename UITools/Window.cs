
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