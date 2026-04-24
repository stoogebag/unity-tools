using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Window))]
public class OpenWindowOnButton : MonoBehaviour
{
    [SerializeField] private bool closeWindow = true;
    [SerializeField] private bool awaitClose = true;

    [SerializeField] private Window windowToOpen;

    [SerializeField] private InputActionProperty inputAction;    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputAction.action.Enable();
        inputAction.action.OnPerformedAsObservable().Subscribe(async t =>
        {
            if(!gameObject.activeInHierarchy) return;
            
            //if (closeParentWindow)
            {
                var window = GetComponent<Window>();
                if (window != null)
                {
                    if (awaitClose)
                        await window.Deactivate();
                    else 
                        window.Deactivate().Forget(); 
                }
            }

            if(windowToOpen != null) await windowToOpen.Activate();
        }).AddTo(this);
    }
}
