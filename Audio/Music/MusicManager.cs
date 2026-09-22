using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        private FMOD.Studio.EventInstance _currentInstance;
        public FMOD.Studio.EventInstance CurrentInstance => _currentInstance;

        private readonly Subject<FMOD.Studio.EventInstance> _instanceStarted = new Subject<FMOD.Studio.EventInstance>();
        public IObservable<FMOD.Studio.EventInstance> InstanceStarted => _instanceStarted.AsObservable();

        private FMOD.GUID _currentEventGuid;

        private CancellationTokenSource _pendingCts;

        private CancellationTokenSource _tempoCts;

        private FMOD.DSP _pitchShiftDsp;
        private readonly CompositeDisposable _tempoSubscriptions = new CompositeDisposable();
        private bool _rampActive;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance != this) return;

            CancelPending();
            CancelPendingTempo();
            StopTempoChange();
            ReleasePitchShiftDsp();
            StopCurrent();
            _tempoSubscriptions.Dispose();
            _instanceStarted.Dispose();
            Instance = null;
        }

        public void SetProfile(MusicProfile profile)
        {
            if (profile == null || IsNull(profile.MusicEvent.Guid))
            {
                StopCurrent();
                return;
            }
            
            Debug.Log("setting music profile " + profile.name, this);

            if (SameGuid(profile.MusicEvent.Guid, _currentEventGuid))
            {
                ApplyParams(profile);
                return;
            }

            StartNew(profile);
        }

        public async UniTask Play(MusicProfile profile, CuePoint when)
        {
            if (profile == null || IsNull(profile.MusicEvent.Guid))
            {
                StopCurrent();
                return;
            }

            if (SameGuid(profile.MusicEvent.Guid, _currentEventGuid))
            {
                ApplyParams(profile);
                return;
            }

            CancelPending();
            var cts = _pendingCts = new CancellationTokenSource();

            if (BeatManager.Instance != null)
            try { await BeatManager.Instance.WaitFor(when, cts.Token); }
            catch (System.OperationCanceledException) { return; }

            StartNew(profile);
        }

        public async UniTask PlayFill(FillProfile fill, CuePoint when)
        {
            if (fill == null || IsNull(fill.FillEvent.Guid)) return;

            CancelPending();
            var cts = _pendingCts = new CancellationTokenSource();

            if (BeatManager.Instance != null)
            try { await BeatManager.Instance.WaitFor(when, cts.Token); }
            catch (System.OperationCanceledException) { return; }

            var fillInstance = RuntimeManager.CreateInstance(fill.FillEvent);
            fillInstance.start();
            fillInstance.release();

            double startTime = Time.unscaledTime;
            float handoff = fill.SecondsUntilNextTrack;
            float keep = Mathf.Clamp(fill.KeepExistingSeconds, 0f, handoff);

            if (keep <= 0f)
            {
                StopCurrent();
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(keep), cancellationToken: cts.Token);
                if (cts.IsCancellationRequested) return;
                StopCurrent();
            }

            float elapsed = (float)(Time.unscaledTime - startTime);
            float remaining = Mathf.Max(0f, handoff - elapsed);
            if (remaining > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(remaining), cancellationToken: cts.Token);
        }

        public void SetPaused(bool paused)
        {
            if (_currentInstance.isValid())
                _currentInstance.setPaused(paused);
        }

        public void SetParam(string name, float value)
        {
            if (_currentInstance.isValid())
                _currentInstance.setParameterByName(name, value);
        }

        public void SetTempo(float bpm, CuePoint when = CuePoint.NextBar)
        {
            if (bpm <= 0f) return;

            CancelPendingTempo();
            if (when == CuePoint.Immediate)
            {
                ApplyTempo(bpm);
                return;
            }

            var cts = _tempoCts = new CancellationTokenSource();
            QueueTempo(() => ApplyTempo(bpm), when, cts);
        }

        public void SetTempo(float bpm, int beats, CuePoint when = CuePoint.NextBar)
        {
            if (bpm <= 0f || beats < 1) return;

            CancelPendingTempo();
            if (when == CuePoint.Immediate)
            {
                StartRamp(bpm, beats);
                return;
            }

            var cts = _tempoCts = new CancellationTokenSource();
            QueueTempo(() => StartRamp(bpm, beats), when, cts);
        }

        private void ApplyTempo(float bpm)
        {
            if (bpm <= 0f) return;

            var provider = BeatManager.Instance?.ActiveProvider as FmodBeatProvider;
            if (provider == null) return;

            float rate = provider.BaseTempo > 0f ? bpm / provider.BaseTempo : 1f;
            if (rate <= 0f) return;

            ApplyPlayRate(rate);
            provider.SetPlayRate(rate);
        }

        private async UniTaskVoid QueueTempo(Action apply, CuePoint when, CancellationTokenSource cts)
        {
            try
            {
                if (BeatManager.Instance == null)
                {
                    apply();
                    return;
                }

                await BeatManager.Instance.WaitFor(when, cts.Token);
                if (cts.IsCancellationRequested) return;

                apply();
            }
            catch (OperationCanceledException) { }
        }

        private void StartRamp(float targetBpm, int beats)
        {
            var provider = BeatManager.Instance?.ActiveProvider as FmodBeatProvider;
            if (provider == null) return;

            _tempoSubscriptions.Clear();

            float startBpm = provider.EffectiveTempo > 0f ? provider.EffectiveTempo : provider.BaseTempo;
            if (startBpm <= 0f) return;

            float stepRatio = Mathf.Pow(targetBpm / startBpm, 1f / beats);
            _rampActive = true;

            int count = 0;
            provider.OnAllBeats
                .Subscribe(_ =>
                {
                    if (!_rampActive) return;
                    count++;
                    float bpm = startBpm * Mathf.Pow(stepRatio, count);
                    ApplyTempo(bpm);
                    if (count >= beats)
                    {
                        _rampActive = false;
                        _tempoSubscriptions.Clear();
                    }
                })
                .AddTo(_tempoSubscriptions);
        }

        private void CancelPendingTempo()
        {
            _tempoCts?.Cancel();
            _tempoCts?.Dispose();
            _tempoCts = null;
        }

        [ButtonGroup("Tempo Test")]
        [Button("½×")]
        private void TestHalfSpeed()
        {
            var provider = BeatManager.Instance?.ActiveProvider as FmodBeatProvider;
            if (provider == null) return;
            SetTempo(provider.BaseTempo * 0.5f, CuePoint.NextBeat);
        }

        [ButtonGroup("Tempo Test")]
        [Button("1×")]
        private void TestNormalSpeed()
        {
            var provider = BeatManager.Instance?.ActiveProvider as FmodBeatProvider;
            if (provider == null) return;
            SetTempo(provider.BaseTempo, CuePoint.NextBeat);
        }

        [ButtonGroup("Tempo Test")]
        [Button("2×")]
        private void TestDoubleSpeed()
        {
            var provider = BeatManager.Instance?.ActiveProvider as FmodBeatProvider;
            if (provider == null) return;
            SetTempo(provider.BaseTempo * 2f, CuePoint.NextBeat);
        }

        public void StopTempoChange()
        {
            CancelPendingTempo();
            _rampActive = false;
            _tempoSubscriptions.Clear();
        }

        public void BindPause(IObservable<bool> pauseStream)
        {
            pauseStream.Subscribe(SetPaused).AddTo(this);
        }

        private void StartNew(MusicProfile profile)
        {
            CancelPendingTempo();
            StopTempoChange();
            ReleasePitchShiftDsp();

            var oldInstance = _currentInstance;

            _currentInstance = RuntimeManager.CreateInstance(profile.MusicEvent);
            _currentEventGuid = profile.MusicEvent.Guid;

            ApplyParams(profile);
            _currentInstance.start();

            EnsurePitchShiftDsp();
            _instanceStarted.OnNext(_currentInstance);

            if (oldInstance.isValid())
            {
                oldInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                oldInstance.release();
                oldInstance.clearHandle();
            }
        }

        private void ApplyParams(MusicProfile profile)
        {
            if (!_currentInstance.isValid()) return;

            foreach (var param in profile.StartParams)
                _currentInstance.setParameterByName(param.Name, param.Value);
        }

        private void StopCurrent()
        {
            CancelPendingTempo();
            StopTempoChange();
            ReleasePitchShiftDsp();

            if (_currentInstance.isValid())
            {
                _currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance.release();
            }

            _currentInstance.clearHandle();
            _currentEventGuid = default;
        }

        private void ApplyPlayRate(float rate)
        {
            if (!_currentInstance.isValid()) return;

            _currentInstance.setPitch(rate);

            EnsurePitchShiftDsp();
            if (_pitchShiftDsp.handle != IntPtr.Zero)
                _pitchShiftDsp.setParameterFloat((int)FMOD.DSP_PITCHSHIFT.PITCH, 1f / rate);
        }

        private void EnsurePitchShiftDsp()
        {
            if (_pitchShiftDsp.handle != IntPtr.Zero) return;
            if (!_currentInstance.isValid()) return;

            if (RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, out _pitchShiftDsp) != FMOD.RESULT.OK)
            {
                _pitchShiftDsp.handle = IntPtr.Zero;
                return;
            }

            _pitchShiftDsp.setParameterFloat((int)FMOD.DSP_PITCHSHIFT.PITCH, 1f);
            _pitchShiftDsp.setParameterInt((int)FMOD.DSP_PITCHSHIFT.FFTSIZE, 2048);
            _pitchShiftDsp.setParameterInt((int)FMOD.DSP_PITCHSHIFT.OVERLAP, 4);

            if (_currentInstance.getChannelGroup(out var group) != FMOD.RESULT.OK)
            {
                ReleasePitchShiftDsp();
                return;
            }

            group.addDSP(0, _pitchShiftDsp);
        }

        private void ReleasePitchShiftDsp()
        {
            if (_pitchShiftDsp.handle != IntPtr.Zero)
            {
                _pitchShiftDsp.release();
                _pitchShiftDsp.handle = IntPtr.Zero;
            }
        }

        private void CancelPending()
        {
            _pendingCts?.Cancel();
            _pendingCts?.Dispose();
            _pendingCts = null;
        }

        private static bool IsNull(FMOD.GUID guid)
        {
            return guid.Equals(default(FMOD.GUID));
        }

        private static bool SameGuid(FMOD.GUID a, FMOD.GUID b)
        {
            return a.Data1 == b.Data1 && a.Data2 == b.Data2 && a.Data3 == b.Data3 && a.Data4 == b.Data4;
        }
    }
}
