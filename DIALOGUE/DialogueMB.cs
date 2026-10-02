#if CINEMACHINE
#if UNITASK
using System;
using System.Collections.Generic;
using EditorTools.Recordable;
using UnityEngine;

public class DialogueMB : MonoBehaviour
{
    public DialogueSpeaker Speaker;

    public List<DialogueLine> Lines = new List<DialogueLine>() { null, null, null, null };
    public RandomSelectionType SelectionType = new RandomSelectionType();
    public DialogueTypes DialogueType;
}

[Serializable]
public class DialogueLine
{
    [Recordable(maxLengthSeconds: 30, transcribeIntoField: nameof(Text))]
    public AudioClip Clip;

    public string Text;
}

public enum DialogueTypes
{
    Cutscene, //this is a dialogue stream.
    Bark,
    Narration,//this is a random dialogue line chosen from the list.
}

#endif
#endif
