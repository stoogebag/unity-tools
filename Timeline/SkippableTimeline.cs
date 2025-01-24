#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Reflection.Editor;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace stoogebag
{
    [RequireComponent(typeof(PlayableDirector))]
    public class SkippableTimeline : MonoBehaviour
    {
        public bool Skippable = true;
        public bool DisablePlayerControl = true;
        public bool CanRunWhileRunning = false; //will this ever be true? play it safe. nfi what this was meant to be lmao

        public bool PlayOnAwake = false;
        
        void Awake()
        {
            Director = GetComponent<PlayableDirector>();
            if (PlayOnAwake)
            {
                Play().Forget();
            }
        }
        
        public static void TrySkip()
        {
            if (CurrentlyPlayingTimeline != null)
            {
                if (CurrentlyPlayingTimeline.State == TimelineState.Paused)
                    CurrentlyPlayingTimeline.SkipToEnd();
            }
        }

        private Playable Playable => Director.playableGraph.GetRootPlayable(0); //cache this? who cares for now


        [Button]
        public async UniTask Play() //should not be launched by anyone except the manager.
        {
            
            if(CurrentlyPlayingTimeline != null)
                Debug.LogError("There is already a timeline playing. You should not be playing another one. maybe in the future...");
            
            CurrentlyPlayingTimeline = this;
            await Director.PlayAndAwait();
            CurrentlyPlayingTimeline = null;
            
//        print("dinished.");
        }

        public static SkippableTimeline CurrentlyPlayingTimeline = null;
        
        public PlayableDirector Director { get; private set; }

        public static void TimelinePause()
        {
            //TimelineSkipBack();
            return;
            CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0)
                .SetSpeed(0d); //BC: we set speed to 0 because calling 'pause' is a bit more like a 'stop' than a pause and causes unwanted behaviour.
            CurrentlyPlayingTimeline.State = TimelineState.Paused;
        }

        private void Update()
        {
            if (CurrentlyPlayingTimeline != null)
            {

                var dialogueClip = GetCurrentClip<DialogueTrack>(CurrentlyPlayingTimeline.Director);
                if(dialogueClip != null){
                    var time = GetNormalisedTime(dialogueClip, CurrentlyPlayingTimeline.Director);
                    
                    if(time > 0.5f && time < 0.9f)
                    {
                        TimelineSkipBack(0.1f);
                        State = TimelineState.Paused;
                    }
                    
                };
            }
        }

        public void SkipToEnd()
        {
            if (CurrentlyPlayingTimeline == null) return;
            var clip = GetCurrentClip<DialogueTrack>(CurrentlyPlayingTimeline.Director);
            CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0).SetTime(clip.end - 0.05f);
            State = TimelineState.Playing;
        }
        
        public static TimelineClip GetCurrentClip<TTrack>(PlayableDirector director, TimelineAsset timelineAsset = null) where TTrack : TrackAsset  
        {
            if(timelineAsset == null) timelineAsset = director.playableAsset as TimelineAsset;
            
            var playable = CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0);
            var time = playable.GetTime();
            var track = timelineAsset.GetOutputTracks().FirstOrDefault(t => t is TTrack) as TTrack;
            var clip = track.GetClips().Where(t => t.start < time && t.end > time).FirstOrDefault();


            return clip;

        }
        
        public static float GetNormalisedTime(TimelineClip clip, PlayableDirector director)
        {
            var playable = CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0);
            var time = playable.GetTime();
            var normalisedTime = (float) (time - clip.start) / (float) (clip.end - clip.start);
            return normalisedTime;
        }
        
        public static float GetRealTimeFromNormalisedTime(TimelineClip clip, PlayableDirector director, float normalisedTime)
        {
            var time = clip.start + (clip.end - clip.start) * normalisedTime;
            return (float) time;
        }
        
        
        public static void TimelineSkipBack(float skipTime)
        {
            var playable = CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0);
            var newTime = playable.GetTime() - skipTime;
            playable.SetTime(newTime);
            CurrentlyPlayingTimeline.State = TimelineState.Paused;

        }

        public void SkipLine()
        {
//        Director.playableGraph.GetRootPlayable(0).SetTime(CurrentClip.end);

//todo
        }


        public void TimelineBack(PlayableDirector director) //todo:figure this one out!
        {
            Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }


        public TimelineState State;

        public enum TimelineState
        {
            Playing,
            Paused, //paused is not really paused. it is in fact doing a tiny loop.
            NotStarted,
            Finished,
        }
    }
}
#endif
#endif
#endif