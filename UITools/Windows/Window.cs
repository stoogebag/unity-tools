#if ODIN_INSPECTOR
#if UNITASK
using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag.UITools.Windows
{
    
    
    
    public class Window : MonoBehaviour
    {
        
        [SerializeField] private Selectable firstSelectedOnActivate;
        [SerializeField] private bool rememberSelectedOnReactivate = true;

        private Selectable _lastSelected = null;
        
        private static readonly Subject<Window> _windowOpened = new Subject<Window>();
        public static IObservable<Window> OnActivatedObservable => _windowOpened.AsObservable();
        private static readonly Subject<Window> _windowClosed = new Subject<Window>();
        public static IObservable<Window> OnDeactivatedObservable => _windowClosed.AsObservable();
        
        
        private IWindowAnimation[] _anims;

        [SerializeField] bool isModal = false;
        [SerializeField] bool closeOnClickOutside = false;


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

        [SerializeField] private bool InitialiseOnStart = false;

        protected virtual void Start()
        {
            if (InitialiseOnStart)
            {
                if (Active == ActiveState.Inactive) Activate();
                else Deactivate();
            }
        }


        public IWindowAnimation[] Animations
        {
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
            if (Active == ActiveState.Activating || Active == ActiveState.Active) return;

            if (isModal)
            {
                CreateModalBlocker();
                //i don't await this. for now we assume that it will be faster than the window activate, it's very quik 
                _blocker.Activate().Forget();
            }
            
            _windowOpened?.Invoke(this);
            
            if (Active == ActiveState.Deactivating)
            {
                //await UniTask.WaitUntil(() => Active != ActiveState.Deactivating); //todo:make an actual cancel!
            }

            Active = ActiveState.Activating;
            gameObject.SetActive(true);

            if (Animations?.Any() != true)
            {
                Active = ActiveState.Active;

                gameObject.SetActive(true);
                return;
            }

            var x = await UniTask.WhenAll(Animations.Select(async t => await t.Activate()));
            if (x.All(t => t))
            {
                Active = ActiveState.Active;
            }
            
            if (firstSelectedOnActivate != null)
            {
                await UniTask.Yield();
                if(_lastSelected != null && rememberSelectedOnReactivate)
                    _lastSelected.Select();
                else
                    firstSelectedOnActivate.Select();
            }
        }

        public void DeactivateImmediate()
        {
            Active = ActiveState.Inactive;
            if(gameObject != null) gameObject.SetActive(false);
        }

        [Button]
        public virtual async UniTask Deactivate()
        {
            if (Active == ActiveState.Inactive || Active == ActiveState.Deactivating) return;

            if (rememberSelectedOnReactivate) // Just store the global selection directly
                _lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            
            if (isModal)
            {
                _blocker.Deactivate().Forget();
            }
            //if (Active == ActiveState.Activating) await UniTask.WaitUntil(() => Active != ActiveState.Activating); //todo:make an actual cancel!

            Active = ActiveState.Deactivating;
            
            //todo: make delay optional.
            // var delay = .5f;
            // await UniTask.Delay(TimeSpan.FromSeconds(delay));
            // if(Active == ActiveState.Activating || Active == ActiveState.Active) return; //this is a cancel?
            
            if (Animations?.Any() != true)
            {
                Active = ActiveState.Inactive;

                gameObject.SetActive(false);
                return;
            }

            var x = await UniTask.WhenAll(Animations.Select(async t => await t.Deactivate()));
            if (x.All(t => t))
            {
                Active = ActiveState.Inactive;

                gameObject.SetActive(false);
                _windowClosed?.Invoke(this);

            }
            

        }

        public ActiveState Active = ActiveState.Inactive;
        private Window _blocker;

        public async UniTask Toggle()
        {
            if (Active == ActiveState.Active || Active == ActiveState.Activating) await Deactivate();
            else await Activate();
        }
        
        
        private void CreateModalBlocker()
        {
            if(_blocker != null) Destroy(_blocker.gameObject);
            // Modal blocker - captures background input
            
            var blockerGO = new GameObject("ModalBlocker");
            blockerGO.transform.SetParent(transform.parent, false);
            
            var windowCanvas = gameObject.GetComponentInAncestor<Canvas>();
            // Window canvas - renders on top
            windowCanvas.overrideSorting = true;
            windowCanvas.sortingOrder = 1000;
            
            
            RectTransform rect = blockerGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = new Vector3(1000, 1000);

            Image image = blockerGO.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.5f); // Nearly transparent
            image.raycastTarget = true;

            _blocker = blockerGO.AddComponent<Window>();
            blockerGO.AddComponent<CanvasGroup>();
            var cgf = blockerGO.AddComponent<CanvasGroupFade>();
            cgf.SetParams(0,0, 0.2f,0.2f);

            
            
            _blocker.transform.SetSiblingIndex(transform.GetSiblingIndex());
            
// Add click handler
            image.OnPointerClickAsObservable().Subscribe(_ =>
            {
                if (closeOnClickOutside)
                {
                    Deactivate();
                }
            }).DisposeWith(this);
        }


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

        public IObservable<TDataModel> ProceedObservable =>
            Observable.FromEvent<TDataModel>(h => Proceed += h, h => Proceed -= h);

        public event Action Cancel;
        public IObservable<Unit> CancelObservable => Observable.FromEvent(h => Cancel += h, h => Cancel -= h);

        public void TryProceed()
        {
            if (VerifyProceed()) Proceed?.Invoke(GetModel());
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
#endif
#endif