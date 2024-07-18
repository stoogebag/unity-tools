// #if TIMELINE
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using stoogebag.Extensions;
// using stoogebag.Utils;
// using UnityEngine;
// using UnityEngine.Playables;
//
// public class TimelinesManager : Singleton<TimelinesManager>
// {
//     private PlayableDirector pausedDirector;
//
//     List<SkippableTimeline> RunningTimelines = new List<SkippableTimeline>();
//     
//     
//
//     public PlayableDirector CameraFadeIn;
//     public PlayableDirector CameraFadeOut;
//
//     public async UniTask PlayTimeline(TimelineLauncher timeline, Action onFinish = null)
//     {
//         await timeline.Director.PlayWithEndCallback(onFinish);
//     }
//     
//     
//     
//     //Called by the TimeMachine Clip (of type Pause)
//     public void PauseTimeline(PlayableDirector whichOne)
//     {
//         activeDirector = whichOne;
//         activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
//         //UIManager.Instance.TogglePressSpacebarMessage(true);
//     }
//
//     //Called by the InputManager
//     public void ResumeTimeline()
//     {
//         //UIManager.Instance.TogglePressSpacebarMessage(false);
//         //UIManager.Instance.ToggleDialoguePanel(false);
//         activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
//         //gameMode = GameMode.Gameplay;
//     }
//
//     public TimelineLauncher DayStartTimeline;
//     private PlayableDirector activeDirector;
// }
//
// public enum TimelineType
// {
//     Cutscene,
//     GameState,
// }
//
// #endif