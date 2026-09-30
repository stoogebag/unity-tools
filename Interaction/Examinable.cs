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

    public void TryExamine(IInteractor interactor)
    {
        OnExamine?.Invoke(interactor);
    }

    /// <summary>
    /// Offer what the interactor could do to this object, for the prompt and for
    /// the upcoming press. Called every frame while focused. The default offers
    /// nothing; override to describe the object's own interactions.
    ///
    /// This is hover-time and must be cheap. It does not commit to anything —
    /// <see cref="IInteraction.CanPerform"/> and the eventual press re-check.
    /// </summary>
    public virtual void OfferInteractions(IInteractor interactor, List<IInteraction> into)
    {
    }

    public string popupName = "name!";
}
#endif
#endif
#endif
