using System;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
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
    
    public float FocusDistance = 10f;
    public float ExamineDistance = 10f;

    string InteractText { get; }

    private void Start()
    {
    }

    public bool CanFocus(IInteractor interactor)
    {
        return interactor.transform.position.DistanceTo(transform.position) < FocusDistance;
    }

    public void Unfocus(IInteractor interactor)
    {
        OnUnfocus?.Invoke(interactor);
        //GetComponentInChildren<UIPopup>(true)?.Deactivate().Forget();
    }

    public void Focus(IInteractor interactor)
    {
        
        OnFocus?.Invoke(interactor);
        //GetComponentInChildren<UIPopup>(true)?.ShowText(popupName).Forget();
    }

    public void TryExamine(IInteractor interactor)
    {
//        print("examining " + gameObject.name);
        OnExamine?.Invoke(interactor);
        //DialogueManager.Instance?.PlayNarration(interactor, ExamineDialogue);
    }

    public string popupName = "name!";
    private DialogueMB _examineDialogue;

    DialogueMB ExamineDialogue
    {
        get
        {
            if (_examineDialogue == null) 
                _examineDialogue = gameObject.FirstOrDefault<DialogueMB>("ExamineDialogue");
            return _examineDialogue;
        }
    }
}