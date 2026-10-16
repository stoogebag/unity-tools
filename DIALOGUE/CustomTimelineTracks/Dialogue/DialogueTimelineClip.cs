#if UNITASK
#if UNIRX
#if CINEMACHINE
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueTimelineClip : PlayableAsset, ITimelineClipAsset, IHoldPoint
{
    public DialogueTimelineBehaviour template = new DialogueTimelineBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public bool WantsHold => template.PauseTimeline;

    public void OnAdvance(PlayableDirector director)
    {
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueTimelineBehaviour>.Create(graph, template);
        var behaviour = playable.GetBehaviour();

        behaviour.Speaker = template.speaker.Resolve(graph.GetResolver());
        if (behaviour.Speaker == null && !string.IsNullOrEmpty(template.dialogueLine))
        {
            Debug.LogWarning($"DialogueTimelineClip: no speaker resolved for line \"{template.dialogueLine}\".", owner);
        }

        return playable;
    }
}
#endif
#endif
#endif
