using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
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
            StopCurrent();
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

        public void BindPause(IObservable<bool> pauseStream)
        {
            pauseStream.Subscribe(SetPaused).AddTo(this);
        }

        private void StartNew(MusicProfile profile)
        {
            var oldInstance = _currentInstance;

            _currentInstance = RuntimeManager.CreateInstance(profile.MusicEvent);
            _currentEventGuid = profile.MusicEvent.Guid;

            ApplyParams(profile);
            _currentInstance.start();

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
            if (_currentInstance.isValid())
            {
                _currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance.release();
            }

            _currentInstance.clearHandle();
            _currentEventGuid = default;
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
