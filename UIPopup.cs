#if UNITASK
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using TMPro;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField] private float FadeInTime = 0.3f;
    [SerializeField] private float FadeOutTime = 0.3f;

    
    [Button]
    public async UniTask Activate()
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        await _canvasGroup.DOFade(1, FadeInTime).ToUniTask();
    }
    
    
    [Button]
    public async UniTask Disable()
    {
        if (!gameObject.activeSelf) return;
        await _canvasGroup.DOFade(0, FadeOutTime).ToUniTask();
        gameObject.SetActive(false);
    }

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
        await Disable();
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