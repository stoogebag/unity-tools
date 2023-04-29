using System;
using stoogebag.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag.UITools.ElementBindingComponents
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
        }
        
        public void Bind(string labelString, ReactiveProperty<int> prop, int min, int max,float stepSize = 1)
        {
            if(label != null) label.text = labelString;

            InitButtons();
            
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

        private void InitButtons()
        {
            if(label == null) label = gameObject.FirstOrDefault<TextMeshProUGUI>("label");
            if(value == null) value = gameObject.FirstOrDefault<TextMeshProUGUI>("value");
            
            if(increment == null) increment = gameObject.FirstOrDefault<Button>("increment");
            if(decrement == null) decrement = gameObject.FirstOrDefault<Button>("decrement");
        }
    }
}