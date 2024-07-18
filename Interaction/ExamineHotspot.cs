#if ADVENTURE_CREATOR
 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AC;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class ExamineHotspot : MonoBehaviour
{
    public string ExamineText = "i'm examining this thing!";
    
    
    [Button]
    void Bind()
    {
        var hs = GetComponent<Hotspot>();
        
        //todo: figure this out! i want good shit.
        //

        var interaction = gameObject.TryGetOrAddComponent<Interaction>();

        if (interaction.actions.Count == 0)
        {
            var examine = ActionSpeech.CreateInstance<ActionSpeech>();
            examine.isPlayer = true;
            examine.messageText = ExamineText;

            interaction.actions.Add(examine);   
        }
        hs.lookButton.interaction = interaction;

    }

    //I HATE THIS. WHY IS IT LIKE THIS?
    private void Start()
    {
        
        var hs = GetComponent<Hotspot>();
        hs.lookButton.interaction = gameObject.GetComponents<Interaction>().FirstOrDefault(t =>
        {
            var a = t.actions[0] as ActionSpeech;
            if (a?.messageText == ExamineText) return true;
            return false;
        });
    }
}
#endif