#if UNITASK
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using TMPro;
using UnityEngine;

public class UIPopup : Window
{
    
    
    public void SetText(string t)
    {
        gameObject.FirstOrDefault<TextMeshProUGUI>().text = t;
    }

    [Button]
    void BarkTest()
    {
        Bark("bark bark!");
    }
    
    public async void Bark(string message, float lingerTime = 1f, float fadeInTime = 0.1f, float fadeOutTime = 1f)
    {
        gameObject.FirstOrDefault<TextMeshProUGUI>().text = message;
        
        await Activate();
        await UniTask.WaitForSeconds(lingerTime);
        await Deactivate();
    }

    public async UniTask StartBark(DialogueLine line)
    {
        gameObject.FirstOrDefault<TextMeshProUGUI>().text = line.Text;
        await Activate();
    }
    public async UniTask ShowText(string message)
    {
        gameObject.FirstOrDefault<TextMeshProUGUI>().text = message;
        await Activate();
    }

    
    
}

#endif