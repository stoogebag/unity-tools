using System;
using EditorTools.Recordable;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public DialogueSpeaker Speaker;

    [Recordable(maxLengthSeconds: 30, transcribeIntoField: nameof(Text))]
    public AudioClip Clip;

    public string Text;

    public float LingerSeconds = 1f;
}
