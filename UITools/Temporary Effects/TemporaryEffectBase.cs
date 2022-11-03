using UniRx;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.UITools.Temporary_Effects
{
    public abstract class TemporaryEffectBase : MonoBehaviour
    {
        protected CompositeDisposable _disposable = new CompositeDisposable();

        public abstract void Activate();
        public abstract void Deactivate();
    }
}