using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace stoogebag.Audio.Music
{
    public enum AnticipationUnit
    {
        Seconds,
        Beats,
    }
    /// <summary>
    /// Shared base for all on-beat behaviours. Subclasses get the multi-beat
    /// CSV subscription ("1,3,4") and anticipation input automatically and
    /// only implement OnBeatTriggered.
    /// </summary>
    public abstract class OnBeatBehaviour : MonoBehaviour
    {
        [SerializeField] protected string beats = "1,2,3,4";
        [FormerlySerializedAs("anticipateSeconds")] [SerializeField] protected float anticipation = 0.2f;
        [SerializeField] protected AnticipationUnit anticipationUnit = AnticipationUnit.Beats;

        protected float LeadTimeSeconds { get; private set; }

        private CompositeDisposable _beatSubscription;
        private bool _started;

        protected virtual void Start()
        {
            _started = true;
            Subscribe();
        }

        protected virtual void OnEnable()
        {
            if (_started)
                Subscribe();
        }

        protected virtual void OnDisable()
        {
            Unsubscribe();
        }

        protected virtual void OnValidate()
        {
            if (!BeatSpec.TryParse(beats, out _, out string error))
                Debug.LogWarning($"[OnBeatBehaviour] {error}", this);
        }

        protected abstract void OnBeatTriggered(BeatData beat);

        protected IBeatProvider ActiveProvider =>
            BeatManager.Instance != null ? BeatManager.Instance.ActiveProvider : null;

        private void Subscribe()
        {
            Unsubscribe();

            if (!BeatSpec.TryParse(beats, out List<float> parsed, out string error))
            {
                Debug.LogWarning($"[OnBeatBehaviour] Not subscribing: {error}", this);
                return;
            }

            var provider = ActiveProvider;
            if (provider == null)
                throw new InvalidOperationException("[OnBeatBehaviour] No active beat provider. Beat behaviours require a registered IBeatProvider.");

            LeadTimeSeconds = anticipationUnit == AnticipationUnit.Beats
                ? anticipation * provider.BeatIntervalSeconds
                : anticipation;

            if (LeadTimeSeconds < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(anticipation),
                    $"[OnBeatBehaviour] Anticipation for '{beats}' produced a negative lead time ({LeadTimeSeconds}s). Lead time must be >= 0.");

            _beatSubscription = new CompositeDisposable();

            Observable.Merge(parsed.Select(b => provider.OnBeatAnticipated(b, LeadTimeSeconds)))
                .Subscribe(OnBeatTriggered)
                .AddTo(_beatSubscription);
        }

        private void Unsubscribe()
        {
            if (_beatSubscription != null)
            {
                _beatSubscription.Dispose();
                _beatSubscription = null;
            }
        }

        /// <summary>
        /// Kill + restart policy: a new beat always kills the in-flight tween
        /// so fast tempos stay on-beat instead of stacking tweens.
        /// </summary>
        protected void RestartTween(ref Tween active, Func<Tween> start)
        {
            if (active != null && active.IsActive())
                active.Kill();

            active = start != null ? start.Invoke() : null;
        }
    }
}
