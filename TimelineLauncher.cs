#if CINEMACHINE
#if TIMELINE
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineLauncher : MonoBehaviour
{
    public DialogueBehaviour CurrentClip;

    

    private void Awake()
    {
        Director = GetComponent<PlayableDirector>();
    }

    [Button]
    public void Launch() //should not be launched by anyone except the manager.
    {
        GetComponent<PlayableDirector>().Play();
    }

    public PlayableDirector Director { get; private set; }
    
    public void PauseTimeline()
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        State = TimelineState.Paused;
    }

    public void SkipLine()
    {
        Director.playableGraph.GetRootPlayable(0).SetTime(CurrentClip.end);
    }
    
    public void TimelinePlay()
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
    
    public void TimelineBack(PlayableDirector director) //todo:figure this one out!
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
    
    
    

    private void Update()
    {
        return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (State == TimelineState.Paused)
            {
                TimelinePlay();
                State = TimelineState.Playing;
                DialogueBox.Instance.Displayed = false;
                CurrentClip = null;
            }
            else if (CurrentClip != null)
            {
                SkipLine();
                CurrentClip = null;
            }
        }
    }

    public TimelineState State;
    public enum TimelineState
    {
        Playing,
        Paused,
        Dialogue
    }
}

#endif
#endif