using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// A per-instance wrapper around one PlayableDirector that plays a timeline and owns its
/// advance/skip, hold points and speed. Registers with PlaybackControls when it has focus so
/// input routes to it.
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class SceneTimeline : MonoBehaviour, IAdvanceable
{
    [SerializeField] private TimelineAsset timelineAsset;
    [SerializeField] private float playbackSpeed = 1f;
    [SerializeField] private bool grabFocus;

    private PlayableDirector _director;
    private readonly Subject<Unit> _onStarted = new Subject<Unit>();
    private readonly Subject<Unit> _onFinished = new Subject<Unit>();
    private bool _fastForward;
    private bool _holding;

    public IObservable<Unit> OnStarted => _onStarted;
    public IObservable<Unit> OnFinished => _onFinished;
    public PlayableDirector Director => _director;
    public TimelineAsset TimelineAsset => timelineAsset;
    public bool IsPlaying => _director != null && _director.state == PlayState.Playing;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        if (_director != null && timelineAsset != null)
            _director.playableAsset = timelineAsset;
        _director.stopped += OnStopped;
    }

    public void Play()
    {
        if (_director == null) return;
        _director.playableAsset = timelineAsset;
        _holding = false;
        _director.Play();
        ApplySpeed();
        if (grabFocus) PlaybackControls.Instance?.Push(this);
        _onStarted.OnNext(Unit.Default);
    }

    public void Pause()
    {
        if (_director != null) _director.Pause();
    }

    public void Stop()
    {
        if (_director == null) return;
        _director.Stop();
        _director.time = 0;
    }

    public double GetTime() => _director != null ? _director.time : 0d;

    public void SetTime(double time)
    {
        if (_director != null) _director.time = time;
    }

    public double GetDuration() => timelineAsset != null ? timelineAsset.duration : 0d;

    public void AssignTimelineAsset(TimelineAsset asset)
    {
        timelineAsset = asset;
        if (_director != null) _director.playableAsset = asset;
    }

    public void SetFastForward(bool on)
    {
        _fastForward = on;
        ApplySpeed();
    }

    public void Advance()
    {
        if (_director == null || !_holding) return;

        var clip = CurrentHoldClip();
        if (clip != null && clip.asset is IHoldPoint hold)
            hold.OnAdvance(_director);

        if (_director.playableGraph.IsValid() && clip != null)
        {
            var root = _director.playableGraph.GetRootPlayable(0);
            if (root.IsValid())
                root.SetTime(clip.start + clip.duration * 0.95);
        }

        Resume();
    }

    public void SkipAll()
    {
        if (_director != null) _director.Stop();
    }

    private void Update()
    {
        if (_director == null || !IsPlaying || _holding) return;

        var clip = CurrentHoldClip();
        if (clip != null && NormalisedTime(clip) >= 0.9f)
        {
            _holding = true;
            ApplySpeed();
        }
    }

    private void Resume()
    {
        _holding = false;
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        if (_director == null || !_director.playableGraph.IsValid()) return;
        var root = _director.playableGraph.GetRootPlayable(0);
        if (!root.IsValid()) return;
        root.SetSpeed(_holding ? 0.0 : playbackSpeed * (_fastForward ? 2.0 : 1.0));
    }

    private void OnStopped(PlayableDirector director)
    {
        _holding = false;
        if (grabFocus) PlaybackControls.Instance?.Pop(this);
        _onFinished.OnNext(Unit.Default);
    }

    private TimelineClip CurrentHoldClip()
    {
        if (timelineAsset == null || _director == null || !_director.playableGraph.IsValid()) return null;
        var root = _director.playableGraph.GetRootPlayable(0);
        if (!root.IsValid()) return null;
        var time = root.GetTime();

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                if (clip.asset is IHoldPoint hp && hp.WantsHold && time >= clip.start && time < clip.end)
                    return clip;
            }
        }

        return null;
    }

    private double NormalisedTime(TimelineClip clip)
    {
        if (_director == null || !_director.playableGraph.IsValid()) return 0.0;
        var root = _director.playableGraph.GetRootPlayable(0);
        if (!root.IsValid() || clip.duration <= 0.0) return 0.0;
        return (root.GetTime() - clip.start) / clip.duration;
    }

    public async UniTask PlayAndAwait()
    {
        var tcs = new UniTaskCompletionSource();
        Action<PlayableDirector> handler = null;
        handler = d => tcs.TrySetResult();
        _director.stopped += handler;
        Play();
        await tcs.Task;
        _director.stopped -= handler;
    }

    private void OnDestroy()
    {
        if (_director != null) _director.stopped -= OnStopped;
        _onStarted.Dispose();
        _onFinished.Dispose();
    }
}
