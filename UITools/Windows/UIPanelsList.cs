using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using stoogebag.UITools.Windows;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelsList : Window
{
    public event Action<bool> OnFinished;
    public IObservable<bool> FinishedObservable => Observable.FromEvent<bool>(h => OnFinished += h, h => OnFinished -= h);
    
    
    public List<Window> Windows = new List<Window>();

    public Button NextButton;
    public Button PreviousButton;
    public Button CloseButton;

    private int currentWindow = 0;

    private void Awake()
    {
        Bind();
    }

    private void Bind()
    {
        _disposable.Clear();
        OpenWindow(0);
        
        NextButton?.OnClickAsObservable().Subscribe(_ =>
        {
            if (currentWindow == Windows.Count - 1)
            {
                Close(true);
            }
            else
            {
                OpenWindow(currentWindow + 1);
            }
        }).AddTo(_disposable);
        
        CloseButton?.OnClickAsObservable().Subscribe(_ =>
        {
            Close(false);
        }).AddTo(_disposable);
        PreviousButton?.OnClickAsObservable().Subscribe(_ =>
        {
            OpenWindow(currentWindow - 1);
        }).AddTo(_disposable);
    }

    private async void OpenWindow(int i)
    {
        if(currentWindow == i) return;
        await Windows[currentWindow].Deactivate();
        currentWindow = i;
        RefreshButtons();
        await Windows[currentWindow].Activate();
    }

    private async UniTask Close(bool finished)
    {
        await Deactivate();
        
        OnFinished?.Invoke(finished);
    }


    private void RefreshButtons()
    {
        SetEnabled(NextButton, true );
        // set up next button changing text etc
        if (currentWindow == Windows.Count - 1)
        {
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = LastNextText;
        }
        else
        {
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = NextText;
        }
        
        SetEnabled(PreviousButton, currentWindow > 0);
        SetEnabled(CloseButton, true);
    }

    private void SetEnabled(Button button, bool b)
    {
        if (button == null) return;
        button.enabled = b;
    }

    [SerializeField] private string PrevText = "back";
    [SerializeField] private string NextText = "next";
    [SerializeField] private string LastNextText = "Finish";
}
