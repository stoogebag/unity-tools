using UniRx;
using UnityEngine;

namespace stoogebag.GameState
{
    public class SourceObjective : Objective
    {
        [SerializeField] private ActivationSource start;
        [SerializeField] private ActivationSource finish;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        private void OnEnable()
        {
            if (start != null)
            {
                start.IsActive
                    .Where(active => active)
                    .First()
                    .Subscribe(_ =>
                    {
                        Begin();
                        WatchFinish();
                    })
                    .AddTo(_subscriptions);
            }
            else
            {
                Begin();
                WatchFinish();
            }
        }

        private void OnDisable()
        {
            _subscriptions.Clear();
        }

        private void WatchFinish()
        {
            if (finish == null)
            {
                return;
            }

            finish.IsActive
                .Where(active => active)
                .First()
                .Subscribe(_ => Complete())
                .AddTo(_subscriptions);
        }

        protected override void OnDestroy()
        {
            _subscriptions.Dispose();
            base.OnDestroy();
        }
    }
}