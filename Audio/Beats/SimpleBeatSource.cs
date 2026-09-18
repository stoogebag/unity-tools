using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class SimpleBeatSource : MonoBehaviour, IBeatProvider
    {
        public float BPM = 120f;

        CompositeDisposable _disposables = new CompositeDisposable();

        BeatData _previousBeat;
        float _lastBeatTime;
        float _lastBarTime;

        public bool StartOnAwake = true;

        public bool IsRunning { get; private set; }
        public float CurrentBeat { get; private set; } = -1f;
        public int CurrentBar { get; private set; } = -1;
        public TimeSignature CurrentTimeSignature { get; private set; }
        public float BeatIntervalSeconds { get; private set; }
        float IBeatProvider.BPM => BPM;

        private void Awake()
        {
            if (StartOnAwake) StartBeats();
        }

        [Button]
        public void StartBeats()
        {
            StopBeats();

            IsRunning = false;
            CurrentBeat = -1f;
            CurrentBar = -1;
            _previousBeat = null;
            _lastBeatTime = 0f;
            _lastBarTime = 0f;

            float interval = GetBeatInterval();

            //todo: timer is probably bad thinking about LAG, pause, etc.
            Observable.Interval(TimeSpan.FromSeconds(interval))
                .Subscribe(_ => EmitBeat())
                .AddTo(_disposables);
        }

        private void EmitBeat()
        {
            int beatsPerBar = 4;

            int barNumber;
            int beatNumber;

            if (_previousBeat == null)
            {
                barNumber = 1;
                beatNumber = 1;
            }
            else
            {
                int previousBeat = Mathf.RoundToInt(_previousBeat.BeatNumber);
                beatNumber = previousBeat >= beatsPerBar ? 1 : previousBeat + 1;
                barNumber = _previousBeat.BarNumber + (beatNumber == 1 ? 1 : 0);
            }

            var timeSignature = new TimeSignature { BeatsPerBar = beatsPerBar, BeatUnit = 4 };

            _lastBeatTime = Time.unscaledTime;
            if (beatNumber == 1) _lastBarTime = _lastBeatTime;

            BeatIntervalSeconds = GetBeatInterval();
            IsRunning = true;
            CurrentBeat = beatNumber;
            CurrentBar = barNumber;
            CurrentTimeSignature = timeSignature;

            var newBeat = new BeatData
            {
                Source = this,
                BarNumber = barNumber,
                BeatNumber = beatNumber,
                CurrentTimeSignature = timeSignature,
            };

            beatSubject.OnNext(newBeat);
            _previousBeat = newBeat;
        }

        [Button]
        public void StopBeats()
        {
            _disposables.Clear();
            IsRunning = false;
            CurrentBeat = -1f;
            CurrentBar = -1;
            _previousBeat = null;
        }

        public float GetBeatInterval()
        {
            return 60f / BPM;
        }

        public float TimeToNextBeat => IsRunning
            ? Mathf.Max(0f, BeatIntervalSeconds - (Time.unscaledTime - _lastBeatTime))
            : -1f;

        public float TimeSinceLastBeat => IsRunning
            ? Time.unscaledTime - _lastBeatTime
            : -1f;

        public float TimeToNextBar => IsRunning
            ? Mathf.Max(0f, TimeToNextBeat + (CurrentTimeSignature.BeatsPerBar - Mathf.RoundToInt(CurrentBeat)) * BeatIntervalSeconds)
            : -1f;

        public float TimeSinceLastBar => IsRunning
            ? Time.unscaledTime - _lastBarTime
            : -1f;

        public IObservable<BeatData> OnAllBeats => beatSubject.AsObservable();

        public IObservable<BeatData> OnBeat(float beatNumber)
        {
            return BeatStream.OnBeat(OnAllBeats, beatNumber, 0f, () => BeatIntervalSeconds, () => CurrentTimeSignature.BeatsPerBar);
        }

        public IObservable<BeatData> OnBeatAnticipated(float beatNumber, float leadTimeInSeconds)
        {
            return BeatStream.OnBeat(OnAllBeats, beatNumber, leadTimeInSeconds, () => BeatIntervalSeconds, () => CurrentTimeSignature.BeatsPerBar);
        }

        public IObservable<BeatData> OnAllBeatsAnticipated(float leadTimeInSeconds)
        {
            return BeatStream.OnAllBeatsAnticipated(OnAllBeats, leadTimeInSeconds, () => BeatIntervalSeconds, () => CurrentTimeSignature.BeatsPerBar);
        }

        private Subject<BeatData> beatSubject = new Subject<BeatData>();
    }
}
