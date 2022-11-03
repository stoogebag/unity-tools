using System;
using TMPro;
using UniRx;
using stoogebag_MonuMental.stoogebag.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.UITools.ElementBindingComponents
{
    public class SpinnerInput : MonoBehaviour
    {

        private CompositeDisposable _disposable = new CompositeDisposable();
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI value;

        [SerializeField] private Button increment;
        [SerializeField] private Button decrement;

        
        private void Awake()
        {
            if(label == null) label = gameObject.FirstOrDefault<TextMeshProUGUI>("label");
            if(value == null) value = gameObject.FirstOrDefault<TextMeshProUGUI>("value");
            
            if(increment == null) increment = gameObject.FirstOrDefault<Button>("increment");
            if(increment == null) decrement = gameObject.FirstOrDefault<Button>("decrement");
        }
        
        public void Bind(string labelString, ReactiveProperty<int> prop, int min, int max,float stepSize = 1)
        {
            label.text = labelString;
        
            _disposable.Clear();
        
            increment.OnClickAsObservable().Subscribe(t =>
            {
                prop.Value = Math.Min(prop.Value + 1, max);
            }).AddTo(_disposable);
            decrement.OnClickAsObservable().Subscribe(t =>
            {
                prop.Value = Math.Max(prop.Value - 1, min);
            }).AddTo(_disposable);


            value.text = prop.Value.ToString();//Math.Round(prop.Value, 1).ToString();
            prop.Subscribe(v => value.text = prop.Value.ToString());//Math.Round(v, 1).ToString());
        }
    
    
    }
}