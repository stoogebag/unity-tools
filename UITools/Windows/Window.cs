using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace stoogebag.UITools.Windows
{
    public class Window : MonoBehaviour
    {
        private IWindowAnimation _anim;

        public event Action OnActivated;
        public event Action OnDeactivated;

        [Button]
        public void ActivateTest()
        {
            Activate();
        }
        [Button]
        public void DeactivateTest()
        {
            Deactivate();
        }
        

        protected CompositeDisposable _disposable = new CompositeDisposable();
    
        [SerializeField]
        private bool InitialiseOnStart = true;

        protected virtual void Start()
        {

            if (InitialiseOnStart)
            {
                if (Active == ActiveState.Inactive) Activate();
                else Deactivate();
            }
        }


        public IWindowAnimation Anim {
            get
            {
                if (_anim == null) _anim = GetComponent<IWindowAnimation>();
                return _anim;
            }
        }
    
        //todo: make this sealed, and fire onActivate and onActivationComplete instead
        public virtual async Task Activate()
        {
            if (Active == ActiveState.Activating || Active == ActiveState.Active) return;
            Active = ActiveState.Activating;

            gameObject.SetActive(true);

            if (Anim == null)
            {
                Active = ActiveState.Active;
                OnActivated?.Invoke();
                return;
            }
            var x = await Anim.Activate();
            if (x)
            {
                Active = ActiveState.Active;
                OnActivated?.Invoke();
            }
        }
        public virtual async Task Deactivate()
        {
            if (Active == ActiveState.Inactive || Active == ActiveState.Deactivating) return;
            Active = ActiveState.Deactivating;
            
            if (Anim == null)
            {
                Active = ActiveState.Inactive;
                OnDeactivated?.Invoke();
                return;
            }
            
            var x = await Anim.Deactivate();
            if (x)
            {
                Active = ActiveState.Inactive;
                OnDeactivated?.Invoke();

                gameObject.SetActive(false);

            }
        }

        public ActiveState Active = ActiveState.Inactive;
    }

    public enum ActiveState
    {
        Active,
        Inactive,
        Activating,
        Deactivating
    }

    public abstract class TemporaryWindow<TInputModel, TDataModel> : Window where TDataModel : class
    {
        private CompositeDisposable _popupDisposable = new CompositeDisposable();
        public async Task<WindowResult> PopupAndAwaitResult(TInputModel inputs, TDataModel data = null)
        {
            Bind(inputs, data);
            await this.Activate();
            var close = new TaskCompletionSource<WindowResult>();

            ProceedObservable.Subscribe(m =>
            {
                close.TrySetResult(new WindowResult()
                {
                    Result = Result.Proceed,
                    Data = m
                });
            }).AddTo(_popupDisposable);

            CancelObservable.Subscribe(m =>
            {
                close.TrySetResult(new WindowResult()
                {
                    Result = Result.Cancel,
                });
            }).AddTo(_popupDisposable);

            var result = await close.Task;
            _popupDisposable.Clear();
            await this.Deactivate(); //possibly dont bother awaiting this...
            return result;
        }


        public event Action<TDataModel> Proceed;
        public IObservable<TDataModel> ProceedObservable => Observable.FromEvent<TDataModel>(h => Proceed += h, h => Proceed -= h);
        public event Action Cancel;
        public IObservable<Unit> CancelObservable => Observable.FromEvent(h => Cancel += h, h => Cancel -= h);

        public void TryProceed()
        {
            if(VerifyProceed()) Proceed?.Invoke(GetModel());
        }
        public void TryCancel()
        {
            if (VerifyCancel()) Cancel?.Invoke();
        }
    
    
        protected virtual bool VerifyProceed()
        {
            return true;
        }

        protected virtual bool VerifyCancel()
        {
            return true;
        }
    
        protected abstract TDataModel GetModel();

        protected abstract void Bind(TInputModel input, TDataModel model = null);
    
        public enum Result
        {
            Proceed,
            Cancel,
            //ForceClose,
        }

        public class WindowResult
        {
            public TDataModel Data;
            public Result Result;
        }
    
    }
}