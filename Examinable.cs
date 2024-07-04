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


    string InteractText { get; }

    private void Start()
    {
    }

    public void Unfocus(IInteractor interactor)
    {
        GetComponentInChildren<UIPopup>(true)?.Deactivate().Forget();
    }

    public void Focus(IInteractor interactor)
    {
        GetComponentInChildren<UIPopup>(true)?.ShowText(popupName).Forget();
    }

    public void TryExamine(IInteractor interactor)
    {
        DialogueManager.Instance?.PlayNarration(interactor, ExamineDialogue);
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