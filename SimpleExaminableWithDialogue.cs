using System;
using System.Collections;
using System.Collections.Generic;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(Examinable))]
[RequireComponent(typeof(DialogueMB))]
public class SimpleExaminableWithDialogue : MonoBehaviour
{
    
    public static event Action<(IInteractor,DialogueMB)> OnExamine;
    public static IObservable<(IInteractor, DialogueMB)> OnExamineObservable => Observable.FromEvent<(IInteractor,DialogueMB)>(h => OnExamine += h, h => OnExamine -= h);


    private void Start()
    {
        GetComponent<Examinable>().OnExamineObservable.Subscribe(interactor =>
        {
            var dialogue = GetComponentInChildren<DialogueMB>();
            //GameManager.Instance.PlayDialogue(interactor, dialogue);
            OnExamine?.Invoke((interactor, dialogue));

        }).DisposeWith(this);
    }
}
