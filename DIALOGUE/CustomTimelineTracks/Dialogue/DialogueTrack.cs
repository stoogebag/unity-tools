using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.903f, 0.87f)]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        
        foreach (var clip in GetClips())
        {
            var myAsset = clip.asset as DialogueClip;
            if (myAsset)
            {
                myAsset.start = clip.start;
                myAsset.end = clip.end;  
            }
        }
        
        
        return base.CreateTrackMixer(graph, go, inputCount);
    }
}