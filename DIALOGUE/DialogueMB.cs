#if CINEMACHINE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using InfinityCode.UltimateEditorEnhancer.UnityTypes;
using UnityEngine.SceneManagement;
#endif
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEditor;

#if NEW_CINEMACHINE
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif
using UnityEngine;

#if UNITY_EDITOR
using Whisper;
#endif
public class DialogueMB : MonoBehaviour
{
    public DialogueSpeaker Speaker;

    //public CinemachineVirtualCamera cam;


    [SerializeField] [ValueDropdown("Devices")]
    public string Device = "Microphone (NVIDIA Broadcast)";

    public List<DialogueLine> Lines = new List<DialogueLine>(){ null, null, null,null};
    public RandomSelectionType SelectionType = new RandomSelectionType();
    public DialogueTypes DialogueType;


    private void Start()
    {
        foreach (var dialogueLine in Lines)
        {
      //      if(dialogueLine.Speaker == null) dialogueLine.Speaker = Speaker;
        }
    }

    


#if UNITY_EDITOR
    private string[] Devices => Microphone.devices;
#else
    private string[] Devices => new string[0];
#endif
    
    public bool AutoTranscribe = true;
}


[Serializable]
public class DialogueLine
{

    public DialogueMB GetParentIfSelected()
    {
#if UNITY_EDITOR
        var dialogueMB = Selection.activeGameObject?.GetComponent<DialogueMB>();
        
        if (dialogueMB == null) return null;
        if (!dialogueMB.Lines.Contains(this)) return null;

        return dialogueMB;
#else
        return null;
#endif
    }
    
    
    [ShowInInspector] public AudioClip Clip;
    public string Text;

    private AudioClip _clip;
    private bool _recording;

    public int MaxClipLength = 30;
    
#if UNITY_EDITOR
    public string Device => GetParentIfSelected()?.Device;
    [ButtonGroup , Button(SdfIconType.Record, "")]
    public void Record()
    {
        _clip = Microphone.Start(Device, false, MaxClipLength, 44100);
        _recording = true;
    }
    
    // [ButtonGroup, Button(SdfIconType.Stop, "")]
    // public void Stop()
    // {
    //     Microphone.End(Device);
    //     _recording = false;
    //     
    //     var clipTrimmed = SavWav.TrimSilence(_clip, 0.001f);
    //     _clip = clipTrimmed;
    //     _clip.name = "name";
    //
    // }
    
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
        
        if(GetParentIfSelected()?.AutoTranscribe == true) Transcribe();
        
    }

    [ButtonGroup, Button(SdfIconType.Pencil, "")]
    public async void Transcribe()
    {
        var manager = GameObjectExtensions.FindObjectOfTypeInActiveScene<WhisperManager>();
        if (manager == null) return;

        await manager.InitModel();
            
        var res = await manager.GetTextAsync(Clip);
        
        if (res == null) 
            Debug.Log("no output text!");

        Text = res.Result.Trim(' ');
    }

    

#endif
    
}

public enum DialogueTypes
{
    Cutscene, //this is a dialogue stream.
    Bark,
    Narration,//this is a random dialogue line chosen from the list.
}

#endif