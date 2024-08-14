
using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR

using InfinityCode.UltimateEditorEnhancer.UnityTypes;
using stoogebag.Extensions;
#endif
using Sirenix.OdinInspector;
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
	
	public static event Action<DialogueLine> DialogueTriggered;
	public static IObservable<DialogueLine> DialogueTriggeredObservable => Observable.FromEvent<DialogueLine>(h =>  DialogueTriggered += h, h => DialogueTriggered -= h);

    public string characterName;
    public string dialogueLine;
    public int dialogueSize;

    public AudioClip Clip;
    private AudioClip _clip;

	public bool hasToPause = false;

	private bool clipPlayed = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
		director = (playable.GetGraph().GetResolver() as PlayableDirector);
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(!clipPlayed
			&& info.weight > 0f)
		{
			
			DialogueTriggered?.Invoke(new DialogueLine()
			{
				Text = dialogueLine,
				Clip = Clip,
			});
			
			// if (GameManager.Instance == null) return;
			// GameManager.Instance.PlayNarrationDialogue( new DialogueLine()
			// {
			// 	Text = dialogueLine,
			// 	Clip = Clip,
			// }).Forget();
			
			
			

			if(Application.isPlaying)
			{
				if(hasToPause)
				{
					pauseScheduled = true;
				}
			}

			clipPlayed = true;
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if(pauseScheduled)
		{
			pauseScheduled = false;
			//GameManager.Instance.PauseTimeline(director);
		}
		else
		{
		//	UIManager.Instance.ToggleDialoguePanel(false);
		}

		clipPlayed = false;
	}

	
	
	
	private bool _recording;

	public int MaxClipLength = 30;
#if UNITY_EDITOR
	public static string Device => Microphone.devices.First(t => t.Contains("NVID"));
	[ButtonGroup , Button(SdfIconType.Record, "")]
	public void Record()
	{
		_clip = Microphone.Start(Device, false, MaxClipLength, 44100);
		_recording = true;
	}
    
    
	[ButtonGroup , Button(SdfIconType.Play, "")]
	public void Play()
	{
		if(Clip != null) PlayClip(Clip);
	}
    
	public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
	{
		AudioUtilsRef.PlayClip(clip);
	}
    
	[ButtonGroup , Button(SdfIconType.Save, "")]
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
	
	
	[ButtonGroup, Button(SdfIconType.Pencil, "")]
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