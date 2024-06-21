#if CINEMACHINE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

#if UNITY6
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif
using UnityEngine;

public class DialogueMB : MonoBehaviour
{
    public Speaker Speaker;

    public CinemachineVirtualCamera cam;

    public List<DialogueLine> Lines;
    public RandomSelectionType SelectionType = new RandomSelectionType();
    public DialogueTypes DialogueType;


    public void Stop()
    {
        
    }

    public void Play()
    {
        print("face it genius, i've been played.");
    }
    
    
}


[Serializable]
public class DialogueLine
{
    [ShowInInspector] public AudioClip Clip;
    public string Text;

    public DialogueSpeaker Speaker;

    public async UniTask Play()
    {
        await Speaker.Play(this);
    }
}

public enum DialogueTypes
{
    Cutscene, //this is a dialogue stream.
    Bark,
    Narration,//this is a random dialogue line chosen from the list.
}

#endif