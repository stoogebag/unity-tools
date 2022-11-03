using System.Threading.Tasks;
using stoogebag_MonuMental.stoogebag.Extensions;
using UniRx;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.UITools.Windows
{
    public class WindowWithClose : Window
    {
        public Button Close;
    
        void Start()
        {
            //base.Start();
            Close = gameObject.FirstOrDefault<Button>("Close");
        }

        public override async Task Activate()
        {
            await base.Activate();
            if(Close == null) Close = gameObject.FirstOrDefault<Button>("Close");

            Close.OnClickAsObservable().Subscribe(a => Deactivate()).AddTo(_disposable);
        }

        public override async Task Deactivate()
        {
            _disposable.Clear();
            await base.Deactivate();
        }

        void Update()
        {
        
        }
    }
}
