#if UNITASK
#if UNIRX
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using UniRx;
using UnityEngine.UI;

namespace stoogebag.UITools.Windows
{
    public class WindowWithClose : Window
    {
        public Button Close;

        protected override void Start()
        {
            base.Start();
            Close = gameObject.FirstOrDefault<Button>("Close");
        }

        public override async UniTask Activate()
        {
            _disposable.Clear();
            await base.Activate();
            if(Close == null) Close = gameObject.FirstOrDefault<Button>("Close");

            Close.OnClickAsObservable().Subscribe(a => Deactivate()).AddTo(_disposable);
        }

        public override async UniTask Deactivate()
        {
            _disposable.Clear();
            await base.Deactivate();
        }
    }
}
#endif
#endif