using System;
using UniRx;
using UnityEngine;

public abstract class TemporaryEffectBase : MonoBehaviour
{
    protected CompositeDisposable _disposable = new();

    public abstract void Activate();
    public abstract void Deactivate();
}