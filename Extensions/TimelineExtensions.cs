#if UNITASK
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace stoogebag.Extensions
{
    public static class TimelineExtensions
    {
        
        public static async UniTask PlayWithEndCallback(this PlayableDirector director, Action endAction)
        {
            director.Play();
            await UniTask.WaitWhile(() => director.state == PlayState.Playing);
            endAction?.Invoke();
        }

        public static async UniTask PlayAndAwait(this PlayableDirector director)
        {
            if(!director.gameObject.activeSelf) director.gameObject.SetActive(true);
            director.Play();
            
            await UniTask.WaitWhile(() =>
            {
                return director.state == PlayState.Playing;
            });
            //endAction?.Invoke();
        }

        public static PlayableDirector GetDirector(this Playable playable)
        {
            return (playable.GetGraph().GetResolver() as PlayableDirector);
        }
        
        
                
        
        public static TimelineClip GetCurrentClip<TTrack>(this PlayableDirector director, TimelineAsset timelineAsset = null) where TTrack : TrackAsset
        {
            if (!director.playableGraph.IsValid()) return null;
            
            if(timelineAsset == null) timelineAsset = director.playableAsset as TimelineAsset;
            
            var playable = director.playableGraph.GetRootPlayable(0);
            var time = playable.GetTime();
            var track = timelineAsset.GetOutputTracks().FirstOrDefault(t => t is TTrack) as TTrack;
            var clip = track.GetClips().Where(t => t.start < time && t.end > time).FirstOrDefault();
            return clip;
        }
        
        public static float GetNormalisedTime(this TimelineClip clip, PlayableDirector director)
        {
            var playable = director.playableGraph.GetRootPlayable(0);
            var time = playable.GetTime();
            var normalisedTime = (float) (time - clip.start) / (float) (clip.end - clip.start);
            return normalisedTime;
        }
        
        public static float GetRealTimeFromNormalisedTime(this TimelineClip clip, PlayableDirector director, float normalisedTime)
        {
            var time = clip.start + (clip.end - clip.start) * normalisedTime;
            return (float) time;
        }

        
    }
}
#endif