#if TIMELINE
using System;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using Application = UnityEngine.Application;

//NOTE! These clips current need to have a GAP between them or things will glitch out with the pause!

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public DialogueLine Line;

    public bool hasToPause = false;

    public double start;
    public double end;
	
    private bool clipPlayed = false;
    private bool pauseScheduled = false;
    private PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //var clip = playable as DialogueClip;
        if(!clipPlayed
           && info.weight > 0f)
        {
            if(Application.isPlaying){
                //		UIManager.Instance.DialogueStart(Line);
                Launcher.CurrentClip = this;

                if(hasToPause)
                {
                    pauseScheduled = true;
                }
            }

            clipPlayed = true;
        }
    }

    private TimelineLauncher Launcher => director.gameObject.GetComponent<TimelineLauncher>();
	
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if(pauseScheduled)
        {
            pauseScheduled = false;
            Launcher.PauseTimeline();
        }
        else
        {
            UIManager.Instance?.DialogueFinished();
        }

        clipPlayed = false;
    }
}
#endif