#if UNITASK
#if CINEMACHINE
using System;
using EditorTools.Recordable;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public static event Action<DialogueBehaviour> DialogueTriggered;
    public static IObservable<DialogueBehaviour> DialogueTriggeredObservable => Observable.FromEvent<DialogueBehaviour>(h => DialogueTriggered += h, h => DialogueTriggered -= h);
    public static event Action<DialogueBehaviour> DialogueEnded;
    public static IObservable<DialogueBehaviour> DialogueEndedObservable => Observable.FromEvent<DialogueBehaviour>(h => DialogueEnded += h, h => DialogueEnded -= h);

    public string speakerName;
    public string dialogueLine;

    public bool PauseTimeline = true;

    [SerializeField]
    [Recordable(maxLengthSeconds: 30, transcribeIntoField: nameof(dialogueLine))]
    public AudioClip Clip;

    private bool clipPlayed = false;
    public PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!clipPlayed && info.weight > 0f)
        {
            DialogueTriggered?.Invoke(this);
            clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (clipPlayed)
        {
            DialogueEnded?.Invoke(this);
            clipPlayed = false;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
    }
}
#endif
#endif
