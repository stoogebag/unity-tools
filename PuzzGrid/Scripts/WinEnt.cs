using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEnt : GridEntity
{
    public override GridActionSetGroup GetGravityMoves()
    {
        return null;
    }


    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary) => null;
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

