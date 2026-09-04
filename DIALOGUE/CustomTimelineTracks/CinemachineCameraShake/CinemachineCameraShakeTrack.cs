using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(SplineAnimatePlayableClip))]
[TrackBindingType(typeof(SplineAnimate))]
public class CinemachineCameraShakePlayableTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CinemachineCameraShakePlayableMixerBehaviour>.Create (graph, inputCount);
    }
}
