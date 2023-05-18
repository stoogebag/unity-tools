using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using stoogebag.UITools.Windows;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelsList : Window
{
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
        
        NextButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (currentWindow == Windows.Count - 1)
            {
                Close();
            }
            else
            {
                OpenWindow(currentWindow + 1);
            }
        }).AddTo(_disposable);
        
        CloseButton.OnClickAsObservable().Subscribe(_ =>
        {
            Close();
        }).AddTo(_disposable);
        PreviousButton.OnClickAsObservable().Subscribe(_ =>
        {
            OpenWindow(currentWindow - 1);
        }).AddTo(_disposable);
    }

    private async void OpenWindow(int i)
    {
        await Windows[currentWindow].Deactivate();
        currentWindow = i;
        RefreshButtons();
        await Windows[currentWindow].Activate();
    }

    private void Close()
    {
        Deactivate();
    }


    private void RefreshButtons()
    {
        SetEnabled(NextButton, true );
        // set up next button changing text etc
        if (currentWindow == Windows.Count - 1)
        {
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        }
        else
        {
            NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
        
        SetEnabled(PreviousButton, currentWindow > 0);
        SetEnabled(CloseButton, true);
    }

    private void SetEnabled(Button button, bool b)
    {
        button.enabled = b;
    }
}
