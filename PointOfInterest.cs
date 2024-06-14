#if CINEMACHINE
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using stoogebag.UITools.Windows;
using UnityEngine;

public class PointOfInterest : MonoBehaviour, IExaminable
{
    public Window UseText;
    public Window BackText;

    public DialogueMB Dialogue;
    
    public CinemachineVirtualCamera Camera;

    [SerializeField] private string _interactText;


    public UIPopup Popup { get; }

    public string InteractText
    {
        get => _interactText;
        set => _interactText = value;
    }

    private void Start()
    {
        Dialogue = GetComponent<DialogueMB>();
        Camera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void OnUnfocus(IInteractor interactor)
    {
    }

    public void OnFocus(IInteractor interactor)
    {
    }


    public void OnTryExamine(IInteractor interactor)
    {
        OnExamine(interactor);
    }

    public void OnExamineCancelled(IInteractor interactor)
    {
        Dialogue?.Stop();
    }

    public void OnExamine(IInteractor interactor)
    {
        Dialogue?.Play();
    }
}



#endif