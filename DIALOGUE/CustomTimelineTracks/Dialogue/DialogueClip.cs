#if UNITASK
#if UNIRX
#if CINEMACHINE
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset, IHoldPoint
{
    public DialogueBehaviour template = new DialogueBehaviour ();
    
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
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, template);
        
        return playable;
    }
}
#endif
#endif
#endif
