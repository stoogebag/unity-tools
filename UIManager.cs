using System.Collections;
using System.Collections.Generic;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using stoogebag.Utils;
using TMPro;
using UniRx;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    
    //[SerializeField] TextMeshProUGUI InteractText;
    [SerializeField] Window InteractWindow;
    
    // Start is called before the first frame update
    void Start()
    {
        var InteractText = InteractWindow.gameObject.GetComponentInDescendants<TextMeshProUGUI>(true);
        //InteractText.gameObject.TryGetComponentInAncestor<Window>(out var ittWindow);
        
        InteractWindow.Deactivate();
        Player.Instance.ObserveEveryValueChanged(t => t.CurrentExaminable).Subscribe(t =>
        {
            if (t == null)
            {
                InteractWindow.Deactivate();
            }
            else
            {
                InteractText.text = t.InteractText;
                InteractWindow.Activate();

            }
            
        }).AddTo(this);
    }

    public void DialogueStart(DialogueMB line)
    {
        
         print($"dialogue  start");
        // DialogueBox.Instance.Displayed = true;
        // DialogueBox.Instance.SetDialogue(line.Lines[0]);
        // line.Speaker.GetAudioSource().clip = line.Clip;
        // line.Speaker.GetAudioSource().Play();
    }

    public void DialogueFinished()
    {
        print("dialogue finished");
        DialogueBox.Instance.Displayed = false;
    }

    public static void Show(CanvasGroup panel)
    {
     
        throw new System.NotImplementedException();
    }
}
