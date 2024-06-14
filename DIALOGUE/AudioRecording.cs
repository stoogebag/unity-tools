using System;
using UnityEngine;

public class AudioRecording : ScriptableObject
{
    public AudioClip clip;
    public DateTime CreationTime;
    public string Description;

    public string guid;
}