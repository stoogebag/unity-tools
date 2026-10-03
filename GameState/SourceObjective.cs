using UniRx;
using UnityEngine;

namespace stoogebag.GameState
{
    /// <summary>
    /// An objective satisfied by an <see cref="ActivationSource"/>. While the
    /// finish source is active it is Complete; when it goes inactive it reverts
    /// to Running, unless <see cref="Objective.oneShot"/> is set.
    /// </summary>
    public class SourceObjective : Objective
    {
        [SerializeField] private ActivationSource start;
        [SerializeField] private ActivationSource finish;

        private readonly CompositeDisposable _subscriptions = new();

        private void OnEnable()
        {
            _subscriptions.Clear();

            if (start != null)
                start.IsActive.Where(active => active).First().Subscribe(_ => Begin()).AddTo(_subscriptions);
            else
                Begin();

            if (finish == null)
                return;

            if (oneShot)
                finish.IsActive.Where(active => active).First().Subscribe(_ => Satisfy()).AddTo(_subscriptions);
            else
                finish.IsActive.Subscribe(active => { if (active) Satisfy(); else Unsatisfy(); }).AddTo(_subscriptions);
        }

        private void OnDisable()
        {
            _subscriptions.Clear();
        }

        protected override void OnDestroy()
        {
            _subscriptions.Dispose();
            base.OnDestroy();
        }
    }
}
