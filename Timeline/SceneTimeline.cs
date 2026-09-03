#if UNITASK
using System;
using Cysharp.Threading.Tasks;
using UniRx;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// SceneTimeline: A scene-based timeline controller that enables visual editing
/// via Unity's Timeline Editor while storing all data directly in the scene.
/// 
/// This component bridges TimelineAsset (for editor editing) with scene serialization,
/// eliminating the need for separate .playable asset files.
/// 
/// Works for both cutscenes and gameplay sequences.
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class SceneTimeline : MonoBehaviour
{
    [SerializeField]
    private TimelineAsset timelineAsset;

    private PlayableDirector _director;

    [SerializeField] float _playbackSpeed = 1f;
    
    public IObservable<Unit> OnFinished => _onFinished;
    private Subject<Unit> _onFinished = new Subject<Unit>();

    private void OnValidate()
    {
        // Ensure PlayableDirector is present and linked
        if (_director == null)
            _director = GetComponent<PlayableDirector>();

        // Auto-create TimelineAsset if it doesn't exist
        if (timelineAsset == null)
            timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();

        if (_director != null && timelineAsset != null)
            _director.playableAsset = timelineAsset;
    }

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        
        _director.played += dir => dir.playableGraph.GetRootPlayable(0).SetSpeed( _playbackSpeed);
        _director.stopped += OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector dir)
    {
        _onFinished.OnNext(Unit.Default);
    }

    /// <summary>
    /// Play the timeline from the beginning.
    /// </summary>
    public void Play()
    {
        if (_director != null)
            _director.Play();
    }

    /// <summary>
    /// Pause the timeline at the current playhead position.
    /// </summary>
    public void Pause()
    {
        if (_director != null)
            _director.Pause();
    }

    /// <summary>
    /// Stop the timeline and reset to the beginning.
    /// </summary>
    public void Stop()
    {
        if (_director != null)
        {
            _director.Stop();
            _director.time = 0;
        }
    }

    /// <summary>
    /// Get the current playback time in seconds.
    /// </summary>
    public double GetTime() => _director != null ? _director.time : 0;

    /// <summary>
    /// Set the playback time in seconds (for scrubbing).
    /// </summary>
    public void SetTime(double time)
    {
        if (_director != null)
            _director.time = time;
    }

    /// <summary>
    /// Get the total duration of the timeline in seconds.
    /// </summary>
    public double GetDuration() => timelineAsset != null ? timelineAsset.duration : 0;

    /// <summary>
    /// Check if the timeline is currently playing.
    /// </summary>
    public bool IsPlaying => _director != null && _director.state == PlayState.Playing;

    /// <summary>
    /// Get the underlying PlayableDirector component.
    /// </summary>
    public PlayableDirector Director => _director;

    /// <summary>
    /// Get the underlying TimelineAsset.
    /// </summary>
    public TimelineAsset TimelineAsset => timelineAsset;

    /// <summary>
    /// Replace the TimelineAsset backing this SceneTimeline and keep the
    /// PlayableDirector in sync. Used by the editor deep-copy / duplicate logic
    /// so each SceneTimeline owns an independent timeline instance.
    /// </summary>
    public void AssignTimelineAsset(TimelineAsset asset)
    {
        timelineAsset = asset;
        if (_director != null)
            _director.playableAsset = asset;
    }

#if UNITASK
    public async UniTask PlayAndAwait()
    {
        var tcs = new UniTaskCompletionSource();
        Action<PlayableDirector> handler = null;
        handler = (d) => { tcs.TrySetResult(); };
        _director.stopped += handler;
        Play();
        await tcs.Task;
        _director.stopped -= handler;
    }
#endif
}
