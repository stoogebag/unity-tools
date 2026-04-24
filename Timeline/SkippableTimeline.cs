
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Febucci.UI.Core;
//using Febucci.UI.Core;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
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

        public bool PlayOnStart = false;
        
        public static bool TimelinePlaying => CurrentlyPlayingTimeline != null;
        
        public static event Action<SkippableTimeline> TimelineStarted;
        public static IObservable<SkippableTimeline> TimelineStartedObservable => Observable.FromEvent<SkippableTimeline>(h =>  TimelineStarted += h, h => TimelineStarted -= h);

        public static event Action<SkippableTimeline> TimelineEnded;
        public static IObservable<SkippableTimeline> TimelineEndedObservable => Observable.FromEvent<SkippableTimeline>(h =>  TimelineEnded += h, h => TimelineEnded -= h);

        
        
        
        void Start()
        {
            Director = GetComponent<PlayableDirector>();
            if (PlayOnStart)
            {
                SkippableTimeline.Play(this).Forget();
            }
        }
        
        public static void TrySkip()
        {
            if (CurrentlyPlayingTimeline != null)
            {
                if (CurrentlyPlayingTimeline.TypingTypewriter != null)
                {
                    CurrentlyPlayingTimeline.TypingTypewriter.SkipTypewriter();
                    CurrentlyPlayingTimeline.TypingTypewriter = null;
                    CurrentlyPlayingTimeline.SkipToPause();
                    return;
                }
                
                
                if (CurrentlyPlayingTimeline.State == TimelineState.Paused)
                    CurrentlyPlayingTimeline.SkipToEnd();
            }
        }

        private Playable Playable => Director.playableGraph.GetRootPlayable(0); //cache this? who cares for now


        [Button]
        public void TryPlayThis()
        {
            Play(this);
        }

        public async UniTask Play()
        {
            await Play(this);
        }
        
        public static async UniTask Play(SkippableTimeline timeline) //should not be launched by anyone except the manager.
        {
            
            if(CurrentlyPlayingTimeline != null)
                Debug.LogError("There is already a timeline playing. You should not be playing another one. maybe in the future...");
            
            
            CurrentlyPlayingTimeline = timeline;
            TimelineStarted?.Invoke(timeline);
            await CurrentlyPlayingTimeline.Director.PlayAndAwait();
            
            TimelineEnded?.Invoke(timeline);
            CurrentlyPlayingTimeline.pausedClips.Clear();
            CurrentlyPlayingTimeline = null;
            
//        print("dinished.");
        }
        
        public static async UniTask Play(string timelineName)
        {
            var timeline = FindObjectsOfType<SkippableTimeline>().FirstOrDefault(t => t.name == timelineName);
            if (timeline == null)
            {
                Debug.LogError($"Timeline {timelineName} not found.");
                return;
            }
            await Play(timeline);
        }

        public static SkippableTimeline CurrentlyPlayingTimeline = null;
        
        public PlayableDirector Director { get; private set; }
        public TypewriterCore TypingTypewriter { get; set; }
        
        HashSet<TimelineClip> pausedClips = new HashSet<TimelineClip>();


        private void Update()
        {
            if (CurrentlyPlayingTimeline != null)
            {
                var dialogueClip = CurrentlyPlayingTimeline.Director.GetCurrentClip<DialogueTrack>();
                if(dialogueClip != null ){
                    var time = dialogueClip.GetNormalisedTime( CurrentlyPlayingTimeline.Director);

                    if ((dialogueClip.asset as DialogueClip).template.PauseTimeline)
                    {
                        if (!pausedClips.Contains(dialogueClip))
                        {

                            if (time > 0.9f)
                            {
                                Pause(dialogueClip);
                            }
                        }
                    }

                };
            }
        }

        private void Pause(TimelineClip dialogueClip)
        {
            CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            pausedClips.Add(dialogueClip);
            //TimelineSkipBack(0.05f);
            State = TimelineState.Paused;
        }

        public void SkipToEnd()
        {
            if (CurrentlyPlayingTimeline == null) return;
            var clip = CurrentlyPlayingTimeline.Director.GetCurrentClip<DialogueTrack>();
            Unpause();
        }
        public void SkipToPause()
        {
            if (CurrentlyPlayingTimeline == null) return;
            var clip = CurrentlyPlayingTimeline.Director.GetCurrentClip<DialogueTrack>();
            CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0)
                .SetTime(clip.start + (clip.end - clip.start) * 0.95f);
            
            Pause(clip);
        }

        public void Unpause() 
        {
            if (CurrentlyPlayingTimeline == null) return;
            CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0).SetSpeed(1);
            State = TimelineState.Playing;
            _unpauseTime = Time.timeSinceLevelLoad;
        }
        
        float _unpauseTime;
        [SerializeField] private float _pauseCooldown = 0.1f;
        
        public static void TimelineSkipBack(float skipTime)
        {
            var playable = CurrentlyPlayingTimeline.Director.playableGraph.GetRootPlayable(0);
            var newTime = playable.GetTime() - skipTime;
            playable.SetTime(newTime);
            CurrentlyPlayingTimeline.State = TimelineState.Paused;

        }

        public TimelineState State;

        public enum TimelineState
        {
            Playing,
            CanSkipToPausePoint,
            Paused, //paused is not really paused. it is in fact doing a tiny loop.
            NotStarted,
            Finished,
        }
    }
}
