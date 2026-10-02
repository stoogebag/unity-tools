using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.GameState
{
    public class SurviveObjective : Objective
    {
        [SerializeField] private float duration = 3f;

        private CancellationTokenSource _cancellation;
        private float _elapsed;

        public override float Progress => duration > 0f ? Mathf.Clamp01(_elapsed / duration) : 0f;
        public override string ProgressText => $"{_elapsed:0.#}/{duration:0.#}";

        private void OnEnable()
        {
            _cancellation = new CancellationTokenSource();
            _elapsed = 0f;
            Begin();
            RunAsync(_cancellation.Token).SuppressCancellationThrow().Forget();
        }

        private void OnDisable()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = null;
        }

        private async UniTask RunAsync(CancellationToken ct)
        {
            var started = Time.time;
            while (true)
            {
                _elapsed = Time.time - started;
                if (_elapsed >= duration)
                {
                    Complete();
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        protected override void OnDestroy()
        {
            OnDisable();
            base.OnDestroy();
        }
    }
}