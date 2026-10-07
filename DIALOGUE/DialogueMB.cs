#if CINEMACHINE
#if UNITASK
using System.Collections.Generic;
using UnityEngine;

public class DialogueMB : MonoBehaviour
{
    public DialogueSpeaker Speaker;

    public List<DialogueLine> Lines = new List<DialogueLine>() { null, null, null, null };
    public RandomSelectionType SelectionType = new RandomSelectionType();
    public DialogueTypes DialogueType;
}

public enum DialogueTypes
{
    Cutscene, //this is a dialogue stream.
    Bark,
    Narration,//this is a random dialogue line chosen from the list.
}

#endif
#endif
