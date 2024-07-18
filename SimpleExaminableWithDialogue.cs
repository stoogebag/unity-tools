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
    private void Start()
    {
        GetComponent<Examinable>().OnExamineObservable.Subscribe(interactor =>
        {
            var dialogue = GetComponentInChildren<DialogueMB>();
            GameManager.Instance.PlayDialogue(interactor, dialogue);


        }).DisposeWith(this);
    }
}
