using System;
using UniRx;
using UnityEngine;

public class Interactable : Examinable
{
    public event Action<IInteractor> OnInteraction;

    public IObservable<IInteractor> OnInteractionObservable =>
        Observable.FromEvent<IInteractor>(h => OnInteraction += h, h => OnInteraction -= h); 

    public event Action<IInteractor> OnInteractionCancelled;
    public IObservable<IInteractor> OnInteractionCancelledObservable =>
        Observable.FromEvent<IInteractor>(h => OnInteractionCancelled += h, h => OnInteractionCancelled -= h); 
    
    void OnTryInteract(IInteractor interactor)
    {
        Interaction(interactor);        
    }
    void InteractionCancelled(IInteractor interactor){} //todo

    void Interaction(IInteractor interactor)
    {
        OnInteraction?.Invoke(interactor);
    }
    
}

public interface IInteractor
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
    bool HasKey(string key);
}