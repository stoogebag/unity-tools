#if TIMELINE


using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MoveAndAwaitClip : PlayableAsset, ITimelineClipAsset
{
    public MoveAndAwaitBehaviour template = new MoveAndAwaitBehaviour ();
    public ExposedReference<TweenToPos> targetPos = new ExposedReference<TweenToPos>();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MoveAndAwaitBehaviour>.Create (graph, template);
        MoveAndAwaitBehaviour clone = playable.GetBehaviour ();
        clone.targetPos = targetPos.Resolve (graph.GetResolver ());
        return playable;
    }
}

#endif