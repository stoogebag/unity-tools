using UnityEngine;

public interface IInteractable : IExaminable
{
    void OnTryInteract(IInteractor interactor);
    void OnInteractionCancelled(IInteractor interactor);
    void OnInteraction(IInteractor interactor);
    
}

public interface IInteractor
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
}


public interface IExaminable
{
    public Transform transform { get; }
    public GameObject gameObject { get; }

    
    string InteractText { get; }

    void OnUnfocus(IInteractor interactor);
    void OnFocus(IInteractor interactor);
    
    void OnTryExamine(IInteractor interactor);

}
