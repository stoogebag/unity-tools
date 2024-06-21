using System;
using Cysharp.Threading.Tasks;
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

    
    public Transform transform { get; }
    public GameObject gameObject { get; }
    
    string InteractText { get; }

    public void Unfocus(IInteractor interactor)
    {
        GetComponentInChildren<UIPopup>(true).Disable().Forget();
    }

    public void Focus(IInteractor interactor)
    {
        GetComponentInChildren<UIPopup>(true).ShowText(popupName).Forget();
    }
    
    public void TryExamine(IInteractor interactor){}

    public string popupName = "name!";
}