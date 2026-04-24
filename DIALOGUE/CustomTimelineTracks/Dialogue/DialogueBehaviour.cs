#if UNITASK
#if ODIN_INSPECTOR

#if CINEMACHINE
using System;
using System.Linq;
using System.Threading;
#if UNITY_EDITOR

//using InfinityCode.UltimateEditorEnhancer.UnityTypes;
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
public class DialogueBehaviour : PlayableBehaviour
{
	
	public static event Action<DialogueBehaviour> DialogueTriggered;
	public static IObservable<DialogueBehaviour> DialogueTriggeredObservable => Observable.FromEvent<DialogueBehaviour>(h =>  DialogueTriggered += h, h => DialogueTriggered -= h);
	public static event Action<DialogueBehaviour> DialogueEnded;
	public static IObservable<DialogueBehaviour> DialogueEndedObservable => Observable.FromEvent<DialogueBehaviour>(h =>  DialogueEnded += h, h => DialogueEnded -= h);

    public string speakerName;
    public string dialogueLine;
    public int dialogueSize;

    public bool PauseTimeline = true;
    
    public AudioClip Clip;
    private AudioClip _clip;

	private bool clipPlayed = false;
	public PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(!clipPlayed
			&& info.weight > 0f)
		{
			DialogueTriggered?.Invoke(this);
			clipPlayed = true;
		}
		else
		{
			//if()
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if(clipPlayed)
		{
			DialogueEnded?.Invoke(this);
			clipPlayed = false;
		}
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		base.OnBehaviourPlay(playable, info);
	}


	private bool _recording;

	public int MaxClipLength = 30;
#if UNITY_EDITOR
	public static string Device => Microphone.devices.First(t => t.Contains("NVID"));
	#if ODIN_INSPECTOR
	[ButtonGroup , Button(SdfIconType.Record, "")]
#endif
	public void Record()
	{
		_clip = Microphone.Start(Device, false, MaxClipLength, 44100);
		_recording = true;
	}
    
#if ODIN_INSPECTOR
	[ButtonGroup , Button(SdfIconType.Play, "")]
#endif
	public void Play()
	{
		if(Clip != null) PlayClip(Clip); 
	}
    
	public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
	{
		//AudioUtilsRef.PlayClip(clip);
		//AudioUtilsRef.PlayClip(clip);
	}
    
	#if ODIN_INSPECTOR
	[ButtonGroup , Button(SdfIconType.Save, "")]
#endif
	public void Save()
	{
		var guid = Guid.NewGuid();
		var wavPath = $"Resources\\audioRecordings\\clip-{guid}";
        
		var clipTrimmed = SavWav.TrimSilence(_clip, 0.001f);
		
		SavWav.Save(wavPath, clipTrimmed);
		Thread.Sleep(10); // Wait for 100 milliseconds
		AssetDatabase.ImportAsset("Assets/" + wavPath + ".wav",  ImportAssetOptions.ForceSynchronousImport);
		Thread.Sleep(10); // Wait for 100 milliseconds
		var myclip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/" + wavPath + ".wav");
		Clip = myclip;
		_recording = false;
		
		SerializedObject editorGUI = new SerializedObject(Selection.activeObject);
 
		// Grab the duration, set and apply modified properties
		SerializedProperty duration = editorGUI.FindProperty("m_Clip.m_Duration");
		duration.doubleValue = (double) Clip.samples / Clip.frequency;
		editorGUI.ApplyModifiedProperties();


	}
	
#if ODIN_INSPECTOR
	[ButtonGroup, Button(SdfIconType.Pencil, "")]
#endif
	public async void Transcribe()
	{
#if WHISPER
		
		var manager = GameObjectExtensions.FindObjectOfTypeInActiveScene<WhisperManager>();
		if (manager == null) return;

		await manager.InitModel();
            
		var res = await manager.GetTextAsync(Clip);
        
		if (res == null) 
			Debug.Log("no output text!");

		dialogueLine = res.Result.Trim(' ');
#else
		Debug.Log("whispter not installed, or directive not set!.");
#endif



	}

	
	

#endif

	
	
	
}
#endif
#endif
#endif