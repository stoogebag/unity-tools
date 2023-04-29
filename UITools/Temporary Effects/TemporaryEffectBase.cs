using UniRx;
using UnityEngine;

namespace stoogebag.UITools.Temporary_Effects
{
    public abstract class TemporaryEffectBase : MonoBehaviour
    {
        protected CompositeDisposable _disposable = new CompositeDisposable();

        public abstract void Activate();
        public abstract void Deactivate();
    }
}