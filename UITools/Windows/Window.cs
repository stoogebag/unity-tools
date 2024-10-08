using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace stoogebag.UITools.Windows
{
    public class Window : MonoBehaviour
    {
        private IWindowAnimation[] _anims;

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
        private bool InitialiseOnStart = false;

        protected virtual void Start()
        {

            if (InitialiseOnStart)
            {
                if (Active == ActiveState.Inactive) Activate();
                else Deactivate();
            }
        }


        public IWindowAnimation[] Animations {
            get
            {
                if (_anims == null) _anims = GetComponents<IWindowAnimation>();
                return _anims;
            }
        }
    
        [Button]
        //todo: make this sealed, and fire onActivate and onActivationComplete instead
        public virtual async UniTask Activate()
        {
            //print($"activating {gameObject.name}");
            if (Active == ActiveState.Activating || Active == ActiveState.Active) return;
            
            //if (Active == ActiveState.Deactivating) await UniTask.WaitUntil(() => Active != ActiveState.Deactivating); //todo:make an actual cancel!
            Active = ActiveState.Activating;

            gameObject.SetActive(true);

            if (Animations?.Any() != true)
            {
                Active = ActiveState.Active;
                
                gameObject.SetActive(true);
                OnActivated?.Invoke();
                return;
            }

            var x = await UniTask.WhenAll(Animations.Select(async t => await t.Activate()));
            if (x.All(t=>t))
            {
                Active = ActiveState.Active;
                OnActivated?.Invoke();
            }
        }
        
        [Button]
        public virtual async UniTask Deactivate()
        {
            //print($"deactivating {gameObject.name}");
            if (Active == ActiveState.Inactive || Active == ActiveState.Deactivating) return;

            //if (Active == ActiveState.Activating) await UniTask.WaitUntil(() => Active != ActiveState.Activating); //todo:make an actual cancel!
            
            Active = ActiveState.Deactivating;
            
            if (Animations?.Any() != true)
            {
                Active = ActiveState.Inactive;
                
                gameObject.SetActive(false);
                OnDeactivated?.Invoke();
                return;
            }

            var x = await UniTask.WhenAll(Animations.Select(async t => await t.Deactivate()));
            if (x.All(t=>t))
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