#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

[RequireComponent(typeof(DialogueTrigger))]
public class TimelineConditionDialogueFinished : TimelineConditionProvider
{

    public override bool ConditionMet()
    {
        var running =GetComponent<DialogueTrigger>().Running; 
        
        return !VD.isActive;
        //return !running;
    }
}
//         block = false;

#endif
#endif
#endif