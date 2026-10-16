#if UNITASK
#if CINEMACHINE
using System;
using EditorTools.Recordable;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueTimelineBehaviour : PlayableBehaviour
{
    public static event Action<DialogueTimelineBehaviour> DialogueTriggered;
    public static IObservable<DialogueTimelineBehaviour> DialogueTriggeredObservable => Observable.FromEvent<DialogueTimelineBehaviour>(h => DialogueTriggered += h, h => DialogueTriggered -= h);
    public static event Action<DialogueTimelineBehaviour> DialogueEnded;
    public static IObservable<DialogueTimelineBehaviour> DialogueEndedObservable => Observable.FromEvent<DialogueTimelineBehaviour>(h => DialogueEnded += h, h => DialogueEnded -= h);

    public ExposedReference<DialogueSpeaker> speaker;
    public string dialogueLine;

    public bool PauseTimeline = true;

    [SerializeField]
    [Recordable(maxLengthSeconds: 30, transcribeIntoField: nameof(dialogueLine))]
    public AudioClip Clip;

    [NonSerialized] public DialogueSpeaker Speaker;
    [NonSerialized] public PlayableDirector director;

    private bool clipPlayed = false;

    public DialogueLine Line => new DialogueLine
    {
        Speaker = Speaker,
        Text = dialogueLine,
        Clip = Clip,
    };

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
