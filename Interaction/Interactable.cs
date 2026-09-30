#if UNIRX
#if CINEMACHINE
using UniRx;
using UnityEngine;

public abstract class Interactable : Examinable
{
    public Subject<InteractionContext> OnInteractPerformed { get; } = new Subject<InteractionContext>();
    public Subject<InteractionContext> OnInteractFailed { get; } = new Subject<InteractionContext>();

    public abstract bool TryInteract(in InteractionContext ctx);

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
#endif
