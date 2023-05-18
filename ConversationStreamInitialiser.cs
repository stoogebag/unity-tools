#if ADVENTURE_CREATOR

using System.Collections;
using System.Collections.Generic;
using AC;
using stoogebag;
using UnityEngine;

public class ConversationStreamInitialiser : MonoBehaviour
{
    private Conversation _conv;

    void Start()
    {
        _conv = GetComponent<Conversation>();
        var streams = GetComponents<ConversationStreamActionList>();
        foreach (var s in streams)
        {
            var currentIndex = 0; ////for now, just initialise. later, maybe handle saving this.
            _conv.options[s.indexInConversation].label = s.Segments[currentIndex].DialogueOption.text;
        }
    }

    void Update()
    {
        
    }
}

#endif