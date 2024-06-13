#if TIMELINE
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using stoogebag.Utils;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinesManager : Singleton<TimelinesManager>
{
    private PlayableDirector pausedDirector;


    public PlayableDirector CameraFadeIn;
    public PlayableDirector CameraFadeOut;

    public async UniTask PlayTimeline(TimelineLauncher timeline, Action onFinish = null)
    {
        await timeline.Director.PlayWithEndCallback(onFinish);
    }

    public TimelineLauncher DayStartTimeline;

}

public enum TimelineType
{
    Cutscene,
    GameState,
}

#endif