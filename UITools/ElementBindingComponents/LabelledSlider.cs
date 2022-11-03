using System;
using stoogebag_MonuMental.stoogebag.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.UITools.ElementBindingComponents
{
    public class LabelledSlider : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI value;
        private Slider slider;

        private void Awake()
        {
            if(label == null) label = gameObject.FirstOrDefault<TextMeshProUGUI>("label");
            if(value == null) value = gameObject.FirstOrDefault<TextMeshProUGUI>("value");
            slider = GetComponentInChildren<Slider>();
        }

        public void Bind(string labelString, ReactiveProperty<float> prop, float min, float max,float stepSize = 1)
        {
            label.text = labelString;

            slider.wholeNumbers = stepSize == 1f;
            slider.minValue = (float)min;
            slider.maxValue = max;
            slider.value = prop.Value;
        
            slider.OnValueChangedAsObservable().Subscribe(v =>
            {
                if (stepSize != 1)
                {
                    float steppedValue = Mathf.Round(v / stepSize) *stepSize;
                    if (steppedValue != v)
                    {
                        slider.value = steppedValue;
                        //Debug.Log(string.Format("New stepped Slider value: {0}", slider.value));
                        return;
                    }
                }
            
                prop.Value = v;
            });
        
            var downButton = gameObject.FirstOrDefault<Button>("labelButton");
            downButton.OnClickAsObservable().Subscribe(t =>
            {
                slider.value = slider.value - stepSize;
            });
            var upButton = gameObject.FirstOrDefault<Button>("valueButton");
            upButton.OnClickAsObservable().Subscribe(t =>
            {
                slider.value = slider.value + stepSize;
            });
        
        
            value.text = Math.Round(prop.Value, 1).ToString();
            prop.Subscribe(v=> value.text = Math.Round(v, 1).ToString());
        }
    }
}