using System;
using UniRx;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.UITools.Selection_and_Input
{
    public class CheckboxInputNav : SelectableUIElement
    {
        private Toggle _toggle;

        private CompositeDisposable _disposable = new CompositeDisposable();
    
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        public override void OnAction(UISelector player)
        {
            base.OnAction(player);
        
            _toggle.isOn = !_toggle.isOn;
        }

        public void Bind(ReactiveProperty<bool> prop, Func<string> labelFunc)
        {
            _disposable.Clear();
            _toggle.isOn = prop.Value;
            _toggle.OnValueChangedAsObservable().Subscribe(tag => prop.Value = tag).AddTo(_disposable);

        }
    }
}
