using System;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class AppearOnBeat : MonoBehaviour
    {
        [SerializeField] private float appearBeat = 1f;
        [SerializeField] private float disappearBeat = 3f;
        [SerializeField] private float anticipateSeconds = 0.2f;

        private readonly Subject<BeatData> _onEnableBeat = new Subject<BeatData>();
        private readonly Subject<BeatData> _onDisableBeat = new Subject<BeatData>();

        public IObservable<BeatData> OnEnableBeat => _onEnableBeat.AsObservable();
        public IObservable<BeatData> OnDisableBeat => _onDisableBeat.AsObservable();

        public bool IsVisible { get; private set; }

        private void Start()
        {
            var provider = BeatManager.Instance.ActiveProvider;
            if (provider == null)
            {
                Debug.LogError("[AppearOnBeat] No active beat provider.", this);
                return;
            }

            ValidateRange(provider);

            provider.OnBeatAnticipated(appearBeat, anticipateSeconds).Subscribe(beat =>
            {
                IsVisible = true;
                _onEnableBeat.OnNext(beat);
            }).AddTo(this);

            provider.OnBeatAnticipated(disappearBeat, anticipateSeconds).Subscribe(beat =>
            {
                IsVisible = false;
                _onDisableBeat.OnNext(beat);
            }).AddTo(this);
        }

        private void ValidateRange(IBeatProvider provider)
        {
            int beatsPerBar = provider.CurrentTimeSignature.BeatsPerBar;
            if (beatsPerBar <= 0) return;

            float max = beatsPerBar + 1f;
            if (appearBeat < 1f || appearBeat >= max)
                Debug.LogWarning($"[AppearOnBeat] appearBeat {appearBeat} outside [1, {max}).", this);
            if (disappearBeat < 1f || disappearBeat >= max)
                Debug.LogWarning($"[AppearOnBeat] disappearBeat {disappearBeat} outside [1, {max}).", this);
        }

        private void OnDestroy()
        {
            _onEnableBeat.Dispose();
            _onDisableBeat.Dispose();
        }
    }
}
