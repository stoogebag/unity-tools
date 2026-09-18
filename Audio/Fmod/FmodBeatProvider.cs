using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class FmodBeatProvider : MonoBehaviour, IBeatProvider
    {
        private readonly Subject<BeatData> _beats = new Subject<BeatData>();
        public IObservable<BeatData> OnAllBeats => _beats.AsObservable();

        private readonly System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();
        private readonly ConcurrentQueue<PendingBeat> _pending = new ConcurrentQueue<PendingBeat>();

        private double _lastBeatAt;
        private double _lastBarAt;
        private float _beatInterval = 0.5f;
        private int _beatsPerBar = 4;

        private FMOD.Studio.EVENT_CALLBACK _callback;
        private FMOD.Studio.EventInstance _attached;
        private GCHandle _handle;

        public bool IsRunning { get; private set; }
        public float CurrentBeat { get; private set; } = -1f;
        public int CurrentBar { get; private set; } = -1;
        public TimeSignature CurrentTimeSignature { get; private set; }
        public float BeatIntervalSeconds => _beatInterval;
        float IBeatProvider.BPM => _beatInterval > 0f ? 60f / _beatInterval : 0f;

        private struct PendingBeat
        {
            public BeatData Data;
            public double TimeSinceStart;
            public float Tempo;
        }

        private void Awake()
        {
            _callback = EventCallback;
        }

        private void Start()
        {
            _watch.Start();

            if (MusicManager.Instance == null)
            {
                Debug.LogError("[FmodBeatProvider] No MusicManager in scene.", this);
                return;
            }

            MusicManager.Instance.InstanceStarted.Subscribe(Attach).AddTo(this);

            if (MusicManager.Instance.CurrentInstance.isValid())
                Attach(MusicManager.Instance.CurrentInstance);
        }

        private void Update()
        {
            while (_pending.TryDequeue(out var pending))
            {
                var beat = pending.Data;

                if (pending.Tempo > 0f) _beatInterval = 60f / pending.Tempo;

                _beatsPerBar = beat.CurrentTimeSignature.BeatsPerBar;
                _lastBeatAt = pending.TimeSinceStart;
                if (Mathf.RoundToInt(beat.BeatNumber) == 1) _lastBarAt = pending.TimeSinceStart;

                IsRunning = true;
                CurrentBeat = beat.BeatNumber;
                CurrentBar = beat.BarNumber;
                CurrentTimeSignature = beat.CurrentTimeSignature;

                _beats.OnNext(beat);
            }
        }

        private void OnDestroy()
        {
            if (_attached.isValid())
            {
                _attached.setCallback(null, 0);
                _attached.clearHandle();
            }

            FreeHandle();
            _beats.Dispose();
        }

        public float TimeToNextBeat => IsRunning
            ? Mathf.Max(0f, (float)(_beatInterval - (_watch.Elapsed.TotalSeconds - _lastBeatAt)))
            : -1f;

        public float TimeSinceLastBeat => IsRunning
            ? (float)(_watch.Elapsed.TotalSeconds - _lastBeatAt)
            : -1f;

        public float TimeToNextBar => IsRunning
            ? Mathf.Max(0f, TimeToNextBeat + (_beatsPerBar - Mathf.RoundToInt(CurrentBeat)) * _beatInterval)
            : -1f;

        public float TimeSinceLastBar => IsRunning
            ? (float)(_watch.Elapsed.TotalSeconds - _lastBarAt)
            : -1f;

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

        private void Attach(FMOD.Studio.EventInstance instance)
        {
            if (_attached.isValid())
            {
                _attached.setCallback(null, 0);
                _attached.clearHandle();
            }

            FreeHandle();

            _attached = instance;

            IsRunning = false;
            CurrentBeat = -1f;
            CurrentBar = -1;
            _lastBeatAt = 0;
            _lastBarAt = 0;
            while (_pending.TryDequeue(out _)) { }

            if (!instance.isValid()) return;

            _handle = GCHandle.Alloc(this);
            instance.setUserData(GCHandle.ToIntPtr(_handle));
            instance.setCallback(_callback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }

        private void FreeHandle()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        private static FMOD.RESULT EventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameters)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            FMOD.RESULT result = instance.getUserData(out IntPtr userData);
            if (result != FMOD.RESULT.OK || userData == IntPtr.Zero)
                return FMOD.RESULT.OK;

            GCHandle handle = GCHandle.FromIntPtr(userData);

            if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED)
            {
                handle.Free();
                return FMOD.RESULT.OK;
            }

            if (handle.Target is FmodBeatProvider provider)
                provider.OnBeat(parameters);

            return FMOD.RESULT.OK;
        }

        private void OnBeat(IntPtr parameters)
        {
            var p = Marshal.PtrToStructure<FMOD.Studio.TIMELINE_BEAT_PROPERTIES>(parameters);

            _pending.Enqueue(new PendingBeat
            {
                TimeSinceStart = _watch.Elapsed.TotalSeconds,
                Tempo = p.tempo,
                Data = new BeatData
                {
                    Source = this,
                    BarNumber = p.bar,
                    BeatNumber = p.beat,
                    CurrentTimeSignature = new TimeSignature
                    {
                        BeatsPerBar = p.timesignatureupper,
                        BeatUnit = p.timesignaturelower,
                    },
                },
            });
        }
    }
}
