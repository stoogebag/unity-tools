#if UNITASK
#if ODIN_INSPECTOR

using System;
using System.Linq;
using System.Threading;
#if UNITY_EDITOR

using stoogebag.Extensions;
#endif
using Sirenix.OdinInspector;
using stoogebag;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if WHISPER
using Whisper;
#endif

[Serializable]
public class SimpleMoveTransformBehaviour : PlayableBehaviour
{
	//
	// public static event Action<SimpleMoveTransformBehaviour> DialogueTriggered;
	// public static IObservable<SimpleMoveTransformBehaviour> DialogueTriggeredObservable => Observable.FromEvent<DialogueBehaviour>(h =>  DialogueTriggered += h, h => DialogueTriggered -= h);
	//
	// public static event Action<SimpleMoveTransformBehaviour> DialogueEnded;
	// public static IObservable<SimpleMoveTransformBehaviour> DialogueEndedObservable => Observable.FromEvent<DialogueBehaviour>(h =>  DialogueEnded += h, h => DialogueEnded -= h);

    public string speakerName;
    public string dialogueLine;
    public int dialogueSize;

    public AudioClip Clip;
    private AudioClip _clip;

	public bool hasToPause = false;
	
	public bool hasPlayed = false;

	private bool clipPlayed = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		base.OnBehaviourPlay(playable, info);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(!clipPlayed
			&& info.weight > 0f)
		{
			//DialogueTriggered?.Invoke(this);
			
			if(Application.isPlaying)
			{
				if(hasToPause)
				{
					pauseScheduled = true;
				}
			}

			clipPlayed = true;
		}
		else
		{
			//if()
		}
	}


	
	


	
	
	
}
#endif
#endif