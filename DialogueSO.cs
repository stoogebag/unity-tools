using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

// [CreateAssetMenu()]
// public class DialogueSO : ScriptableObject, IDialogue
// {
//     public Speaker Speaker;
//     public AudioClip Clip;
//     public string Text;
//
//     public CinemachineVirtualCamera cam;
//
//     public List<DialogueLine> Lines;
//     public RandomSelectionType SelectionType;
//     
//     
//     public void Stop()
//     {
//         
//     }
//
//     public void Play()
//     {
//         
//     }
// }

public interface IDialogue
{
    public Speaker Speaker { get; }

    public CinemachineVirtualCamera cam { get; }

    public List<DialogueLine> Lines { get; } //clips means multiple audio clips, not in sequence but various takes.
    
    public RandomSelectionType SelectionType { get; }
    public DialogueTypes DialogueType { get; }
    
    void Stop();
    void Play();
}

[Serializable]
public class DialogueLine
{
    [ShowInInspector] public AudioClip Clip;
    public string Text;
}

public enum DialogueTypes
{
    Series, //this is a dialogue stream.
    Random, //this is a random dialogue line chosen from the list.
}