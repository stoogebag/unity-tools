using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class FloorEnt : GridEntity
{
    public override GridActionSetGroup GetGravityMoves()
    {
        return null;
    }



    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary) => null;
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;
    
    
    public override GridActionConsequences GetConsequences(GridAction action)
    {
        if (action is IPushAction move)
        {
            return GridActionConsequences.ActionFailed;
        }
        return GridActionConsequences.ActionApproved;
        
    }
}
