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
    public TextMeshProUGUI textValue;
    public TextMeshProUGUI textLabel;

    event Action OnRefresh;

    private Func<string, bool> IsAvailableFunc;
    private Func<string> _labelFunc;
    private CompositeDisposable _disposable = new();

    public IObservable<Unit> RefreshObservable => Observable.FromEvent(h => OnRefresh += h, h => OnRefresh -= h);
    
    private void TryChangeIndex(int i)
    {
        if (OptionsArray.Length == 0) return; 
        while (true)
        {
            index = (index + i +  OptionsArray.Length) % OptionsArray.Length;
            if (IsAvailableFunc == null) break;
            if (IsAvailableFunc(OptionsArray[index])) break;
        }

        Refresh();
    }

    public void Refresh()
    {
        if(textValue != null) textValue.text = option;
        if(textLabel != null) textLabel.text = _labelFunc();
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

    public void Bind(Func<string> getValue, Action<string> setValue, Func<string> getLabelFunc,string[] options, string selected, Func<string, bool> availabilityFunc = null)
    {
        _disposable.Clear();
        
        if(textLabel != null) textLabel.text = getLabelFunc();
        _labelFunc = getLabelFunc;
        OptionsArray = options;
        IsAvailableFunc = availabilityFunc;
        SetOption(selected); 
        
        back?.OnClickAsObservable().Subscribe(t =>
        {
            TryChangeIndex(-1);
        }).AddTo(_disposable);
        forward?.OnClickAsObservable().Subscribe(t =>
        {
            TryChangeIndex(1);
        }).AddTo(_disposable);
    }
    
    public void Bind<TEnum>(Func<TEnum> getValue, Action<TEnum> setValue, Func<string> getLabelFunc, TEnum selected, Func<TEnum, bool> availabilityFunc = null) where TEnum: struct, System.Enum
    {
        Bind(() => getValue().ToString(), s => setValue(Enum.Parse<TEnum>(s)), getLabelFunc,
            Enum.GetNames(typeof(TEnum)), Enum.GetName((typeof(TEnum)), selected));
    }
    
    public void Bind<TEnum>(ReactiveProperty<TEnum> prop, Func<string> getLabelFunc, TEnum selected, Func<TEnum,bool> availabilityFunc) where TEnum:struct, System.Enum
    {
        Bind<TEnum>(() => prop.Value, v => prop.Value = v, getLabelFunc, selected, availabilityFunc);
    }

    public void Bind<T>(ReactiveProperty<T> prop, Func<T, string> toString, Func<string, T> fromString, string[] options, Func<string> getLabelFunc, T selected, Func<T, bool> availabilityFunc)
    {
        Bind(() => toString(prop.Value), s=> prop.Value = fromString(s), getLabelFunc, options, toString(selected), s=> availabilityFunc(fromString(s)) );
    } 
}
