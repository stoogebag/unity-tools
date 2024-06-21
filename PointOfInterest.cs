#if UNITASK
#if CINEMACHINE
using System.Collections;
using System.Collections.Generic;

#if UNITY6
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif
using stoogebag.UITools.Windows;
using UnityEngine;

[RequireComponent(typeof(Examinable))]
public class PointOfInterest : MonoBehaviour 
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
        print($"unfocused off {gameObject.name}");
    }

    public void OnFocus(IInteractor interactor)
    {
        print($"focused on {gameObject.name}");
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
#endif