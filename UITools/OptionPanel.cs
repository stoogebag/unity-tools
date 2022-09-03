using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using TMPro;
using UniRx;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{


    public string[] OptionsArray;

    public int index;
    public string option => OptionsArray[index];

    public Button back;
    public Button forward;
    public TextMeshProUGUI text;
    public TextMeshProUGUI textLabel;

    event Action OnRefresh;

    private Func<string, bool> IsAvailableFunc;
    private Func<string> _labelFunc;

    public IObservable<Unit> RefreshObservable => Observable.FromEvent(h => OnRefresh += h, h => OnRefresh -= h);
    
    void Start()
    {
    }

    public void Refresh()
    {
        text.text = option;
        textLabel.text = _labelFunc();
        OnRefresh?.Invoke();
    }

    public void SetOption(string c)
    {
        index = OptionsArray.IndexOfFirst(t => t == c);
        if (index == -1) index = 0;
        // while (true)
        // {
        //     if (IsAvailableFunc(option)) break;
        //     index++;
        // }

        //i did this already, and won't send a used colour.
        
        Refresh();
    }

    public void Bind(Func<string> getLabelFunc,string[] options, string selected, Func<string, bool> availabilityFunc = null)
    {
        textLabel.text = getLabelFunc();
        _labelFunc = getLabelFunc;
        OptionsArray = options;
        IsAvailableFunc = availabilityFunc;
        SetOption(selected);
    }
}
