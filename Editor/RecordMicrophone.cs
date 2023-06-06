using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

#if ADVENTURE_CREATOR
using AC;
#endif

public class RecordMicrophone : MonoBehaviour
{
    [Button]
    public void Record()
    {
        _clip = Microphone.Start(Device, false, MaxClipLength, 44100);
        _recording = true;
    }
    
    [Button]
    public void Stop()
    {
        Microphone.End(Device);
        _recording = false;
    }

    [Button]
    public void Play()
    {
        if(_clip != null) GetComponent<AudioSource>().PlayOneShot(_clip);
    }

    [Button]
    public void Save()
    {
        var res = AudioRecording.CreateInstance<AudioRecording>();
        
        res.guid = Guid.NewGuid().ToString();
        var wavPath = $"Resources/audioRecording-clip-{res.guid}";
     
        
        SavWav.Save(wavPath, _clip);

        return;
        res.clip = _clip;
        res.CreationTime = DateTime.UtcNow;
        res.Description = "!";

        
        AssetDatabase.CreateAsset(res.clip, $"Assets/Resources/audioRecording-clip-{res.guid}.asset");
        AssetDatabase.CreateAsset(res, $"Assets/Resources/audioRecording-{res.guid}.asset");
        
        //var clip = SavWav.TrimSilence(_clip, )

        
    }

#if ADVENTURE_CREATOR
    [Button]
    void GetLines()
    {
        var interactions = FindObjectsOfType<Interaction>();
        
        var lines = interactions.SelectMany(t => t.actions).Where(t=>t != null).Where(t => t?.Category == ActionCategory.Dialogue).Cast<ActionSpeech>();

        Lines = lines.Select(t => new DialogueLineClip()
        {
            name = t.Title,
            line = t.messageText,
            //path = t.speaker
            
            
        }).ToList();
    }
#endif

    private bool _recording;
    public int MaxClipLength = 30;

    public List<DialogueLineClip> Lines;


    [ValueDropdown("Devices")]
    public string Device;

    private AudioClip _clip;

    public string[] Devices => Microphone.devices;


}
[Serializable]
public class DialogueLineClip
{
    public string name;
    public AudioClip clip;
    public string line;
    public string path;
    
    [Button]
    public void Record()
    {
        //_clip = Microphone.Start(Device, false, MaxClipLength, 44100);
        //_recording = true;
    }
}
