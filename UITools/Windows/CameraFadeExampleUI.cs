using System.Collections;
using System.Collections.Generic;
using stoogebag.UITools.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class CameraFadeExampleUI : MonoBehaviour
{
    public Button fadeInOutButton;
    public Window FadePanel;

    // Start is called before the first frame update
    void Start()
    {
        fadeInOutButton.OnClickAsObservable().Subscribe(async t=>
        {
            print(FadePanel.Active);
            if (FadePanel.Active == ActiveState.Activating ||FadePanel.Active == ActiveState.Active) await FadePanel.Deactivate();
            else await FadePanel.Activate();
        });
    }
}
