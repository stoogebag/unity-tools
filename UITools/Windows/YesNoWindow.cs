using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.UI;

namespace stoogebag.UITools.Windows
{
    public class YesNoWindow : Window
    {

        public Button Yes;
        public Button No;

        public void Bind(Action onSuccess, Action onCancel = null, Func<bool> validateYes = null , Func<bool> validateNo = null )
        {
            _onSuccess = onSuccess;
            _onCancel = onCancel;
            _validateYes = validateYes ?? (() =>true) ;
            _validateNo = validateNo ?? (() =>true) ;
        }

        public override async Task Activate()
        {
            await base.Activate();

            No.OnClickAsObservable().Subscribe(a =>
            {
                if (_validateNo())
                {
                    Deactivate();
                    _onCancel?.Invoke();
                }
            }).AddTo(_disposable);
        
            Yes.OnClickAsObservable().Subscribe(a =>
            {
                if (_validateYes())
                {
                    Deactivate();
                    _onSuccess?.Invoke();
                }
            }).AddTo(_disposable);
        }

        private Action _onSuccess;
        private Action _onCancel;
        private Func<bool> _validateYes;
        private Func<bool> _validateNo;

        public override async Task Deactivate()
        {
            _disposable.Clear();
            await base.Deactivate();
        }

    
    
    
    }
}