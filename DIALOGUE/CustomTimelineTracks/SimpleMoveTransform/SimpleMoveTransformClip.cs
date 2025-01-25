#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SimpleMoveTransformClip : PlayableAsset, ITimelineClipAsset
{
    public SimpleMoveTransformBehaviour template = new SimpleMoveTransformBehaviour ();

    public double start;
    public double end;

    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SimpleMoveTransformBehaviour>.Create(graph, template);
        // playable.GetBehaviour().start = start;
        // playable.GetBehaviour().end = end;
        
        return playable;
    }
    
    
    
}
#endif
#endif
#endif