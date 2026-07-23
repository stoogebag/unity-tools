#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEnt : GridEntity
{
    
    public override bool CanMove => false;
    public override GridActionSetGroup GetGravityMoves()
    {
        return null;
    }


    public override IEnumerable<GridActionSet> GetSettlementMoves(GridActionSummary actionSummary)
    {
        yield break;
        
    }
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;
    
    
    public override GridActionConsequences GetConsequences(GridAction action)
    {
        // if (action is IPushAction move)
        // {
        //     return (false, null);
        // }
        return GridActionConsequences.ActionApproved;
    }
}

#endif
#endif
#endif