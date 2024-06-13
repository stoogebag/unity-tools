#if UNITASK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public float OpenTime = 0.5f;
    public float CloseTime = .3f;
    private bool IsOpen;


    private void Start()
    {
        //SetPopupText();
    }

    private bool inMotion;
    public GameObject doorGO;

    private async void Open()
    {
        IsOpen = true;
        inMotion = true;
        await doorGO.transform.DOLocalRotate(new Vector3(0,95f,0), OpenTime)
            .ToUniTask();
        inMotion = false;
        Opened();
    }


    private async void Close()
    {
        IsOpen = false;
        inMotion = true;
        await doorGO.transform.DOLocalRotate(new Vector3(0,0,0), CloseTime)
            .ToUniTask();

        Closed();
        inMotion = false;
    }

    private void Closed()
    {
    }

    private void Opened()
    {
    }


    public bool BlocksMovement => !IsOpen;
    public UIPopup Popup => GetComponentInChildren<UIPopup>(true);

    public string InteractText => IsOpen ? "Close" : "Open"; 

    public void OnUnfocus(IInteractor interactor)
    {
    }

    public void OnFocus(IInteractor interactor)
    {
    }

    public void OnTryExamine(IInteractor interactor)
    {
        throw new NotImplementedException();
    }

    public void OnTryInteract(IInteractor interactor)
    {
        if (inMotion) return;
        if (IsOpen) Close();
        else Open();
    }

    public void OnInteractionCancelled(IInteractor interactor)
    {
    }

    public void OnInteraction(IInteractor interactor)
    {
        throw new NotImplementedException();
    }
}

#endif