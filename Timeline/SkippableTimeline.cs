using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class SkippableTimeline : MonoBehaviour
{
    public bool Skippable = true;
    public bool DisablePlayerControl = true;
    public bool CanRunWhileRunning = false; //will this ever be true? play it safe 
    
    
    public void TryPlay()
    {
        
    }

    private Playable Playable => Director.playableGraph.GetRootPlayable(0); //cache this? who cares for now
    
    private void Awake()
    {
        Director = GetComponent<PlayableDirector>();
    }

    [Button]
    public async UniTask Play() //should not be launched by anyone except the manager.
    {
        await Director.PlayAndAwait();
//        print("dinished.");
    }

    public PlayableDirector Director { get; private set; }
    
    public void TimelinePause()
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(0d); //BC: we set speed to 0 because calling 'pause' is a bit more like a 'stop' than a pause and causes unwanted behaviour.
        State = TimelineState.Paused;
    }

    public void SkipLine()
    {
//        Director.playableGraph.GetRootPlayable(0).SetTime(CurrentClip.end);

//todo
    }
    
    public void TimelinePlay()
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
    
    public void TimelineBack(PlayableDirector director) //todo:figure this one out!
    {
        Director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
    
    
    public void Skip()
    {
        //for now, just skip the whole thing. in future, add the 
        //power to skip chunks etc, or force a final section, for example a fadeout.
        //or to apply something like a fadeout before actually applying the skip, etc.
    }
    
    public TimelineState State;
    public enum TimelineState
    {
        Playing,
        Paused,
        NotStarted,
        Finished,
    }
}
