using System;
using System.Collections;
using System.Collections.Generic;
using stoogebag.Extensions;
using stoogebag.Utils;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueManager : Singleton<DialogueManager>
{
    
    public StringAudioSourceDictionary AudioSourcesDic;

}

[Serializable]
public class StringAudioSourceDictionary : UnitySerializedDictionary<string, AudioSource> { }
