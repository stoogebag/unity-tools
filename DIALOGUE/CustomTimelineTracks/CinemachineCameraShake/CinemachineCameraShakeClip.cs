using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CinemachineCameraShakePlayableClip : PlayableAsset, ITimelineClipAsset
{
    public CinemachineCameraShakePlayableBehaviour template = new CinemachineCameraShakePlayableBehaviour ();

    //public ExposedReference<SplineContainer> splineContainer;
    public float shakeAmplitude = 1f;
    
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.All; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CinemachineCameraShakePlayableBehaviour>.Create (graph, template);
        CinemachineCameraShakePlayableBehaviour clone = playable.GetBehaviour ();
        clone.shakeAmplitude = shakeAmplitude;//.Resolve(graph.GetResolver());
        return playable;
    }
    
}
