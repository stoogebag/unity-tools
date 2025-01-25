using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0f, 0.7564783f)]
[TrackClipType(typeof(MoveAndAwaitClip))]
[TrackBindingType(typeof(Transform))]
public class MoveAndAwaitTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MoveAndAwaitMixerBehaviour>.Create (graph, inputCount);
    }
}
