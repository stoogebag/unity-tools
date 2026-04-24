using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OpenWindowOnClick : MonoBehaviour
{
    [SerializeField] private bool closeParentWindow = true;
    [SerializeField] private bool awaitClose = true;

    [SerializeField] private Window windowToOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().OnClickAsObservable().Subscribe(async t =>
        {
            if (closeParentWindow)
            {
                var parent = gameObject.GetComponentInAncestor<Window>();
                if (parent != null)
                {
                    if (awaitClose)
                        await parent.Deactivate();
                    else 
                        parent.Deactivate().Forget(); 
                }
            }

            if(windowToOpen != null) await windowToOpen.Activate();
        }).AddTo(this);
    }
}