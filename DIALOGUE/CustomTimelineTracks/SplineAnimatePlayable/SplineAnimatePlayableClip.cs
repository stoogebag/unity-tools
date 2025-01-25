using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;

[Serializable]
public class SplineAnimatePlayableClip : PlayableAsset, ITimelineClipAsset
{
    public SplineAnimatePlayableBehaviour template = new SplineAnimatePlayableBehaviour ();

    public ExposedReference<SplineContainer> splineContainer;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.All; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SplineAnimatePlayableBehaviour>.Create (graph, template);
        SplineAnimatePlayableBehaviour clone = playable.GetBehaviour ();
        clone.splineContainer = splineContainer.Resolve(graph.GetResolver());
        return playable;
    }
    
}
