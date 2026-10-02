#if UNIRX
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class Interactable : Examinable
{
    public Subject<InteractionContext> OnInteractPerformed { get; } = new Subject<InteractionContext>();
    public Subject<InteractionContext> OnInteractFailed { get; } = new Subject<InteractionContext>();

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

    protected virtual void OnDestroy()
    {
        OnInteractPerformed.Dispose();
        OnInteractFailed.Dispose();
    }
}

public interface IInteractor
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
    bool HasKey(string key);
}
#endif
