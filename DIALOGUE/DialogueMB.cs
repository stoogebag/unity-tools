#if CINEMACHINE
using System;
using System.Collections;
using System.Collections.Generic;

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

#endif