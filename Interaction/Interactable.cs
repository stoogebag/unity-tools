#if UNIRX
#if CINEMACHINE
using System;
using UniRx;
using UnityEngine;

public class Interactable : Examinable
{
    public event Action<IInteractor> OnInteraction;


    public float InteractDistance = 5f;

    public IObservable<IInteractor> OnInteractionObservable =>
        Observable.FromEvent<IInteractor>(h => OnInteraction += h, h => OnInteraction -= h); 

    public event Action<IInteractor> OnInteractionCancelled;
    public IObservable<IInteractor> OnInteractionCancelledObservable =>
        Observable.FromEvent<IInteractor>(h => OnInteractionCancelled += h, h => OnInteractionCancelled -= h); 
    
    public void TryInteract(IInteractor interactor)
    {
        if (InteractDistance < Vector3.Distance(interactor.transform.position, transform.position)) return;
        Interact(interactor);        
    }

    void InteractionCancelled(IInteractor interactor)
    {
        //OnInera
        
    } //todo

    void Interact(IInteractor interactor)
    {
        //todo: figure out the right way to do this. it seems jank to hand responsibility back and forth like this,
        //but i don't want to have to sub to a bunch of shit.
        //but sometimes the interactor is the guy who ought to handle things, other times the interactable....
        //eg a door can open itself. but a 'inspectable' probs should be handled by a central authority (eg player obj)...
        
        OnInteraction?.Invoke(interactor);
        Debug.Log($"interacted! with {gameObject.name}", gameObject);
        interactor.Interacted(this);
    }

}

public interface IInteractor
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
    bool HasKey(string key);
    void Interacted(Interactable interactable);
}

#endif
#endif