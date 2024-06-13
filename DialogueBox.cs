using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stoogebag.Utils;
using UnityEngine;

public class DialogueBox : Singleton<DialogueBox>
{
    [field: SerializeField]
    public bool Displayed
    {
        get => _displayed;
        set
        {
            if (_displayed == value) return;
            _displayed = value;
            OnDisplayedChanged?.Invoke(_displayed);
        }
    }

    public event Action<bool> OnDisplayedChanged;

    
    public DialogueLine Line;
    private bool _displayed;
    public event Action<DialogueLine> OnLineChanged;
    
    [Button]
    public void SetDialogue(DialogueLine line)
    {
        if (Line == line) return;
        Line = line;
        OnLineChanged?.Invoke(line);
    }
}
