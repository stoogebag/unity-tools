#if UNITASK
#if UNIRX
#if CINEMACHINE
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Examinable : MonoBehaviour
{
    public event Action<IInteractor> OnFocus;

    public IObservable<IInteractor> OnFocusObservable =>
        Observable.FromEvent<IInteractor>(h => OnFocus += h, h => OnFocus -= h);

    public event Action<IInteractor> OnUnfocus;

    public IObservable<IInteractor> OnUnfocusObservable =>
        Observable.FromEvent<IInteractor>(h => OnUnfocus += h, h => OnUnfocus -= h);

    public event Action<IInteractor> OnExamine;

    public IObservable<IInteractor> OnExamineObservable =>
        Observable.FromEvent<IInteractor>(h => OnExamine += h, h => OnExamine -= h);

    public event Action<IInteractor> OnUnExamine;

    public IObservable<IInteractor> OnUnExamineObservable =>
        Observable.FromEvent<IInteractor>(h => OnUnExamine += h, h => OnUnExamine -= h);

    string InteractText { get; }

    public void Unfocus(IInteractor interactor)
    {
        OnUnfocus?.Invoke(interactor);
    }

    public void Focus(IInteractor interactor)
    {
        OnFocus?.Invoke(interactor);
    }

    /// <summary>
    /// Called by the interactor when the examine action fires on this object.
    /// Examine is its own channel: it is not offered, and cannot be suppressed.
    /// </summary>
    public void TryExamine(IInteractor interactor)
    {
        OnExamine?.Invoke(interactor);
    }

    public string popupName = "name!";
}
#endif
#endif
#endif
