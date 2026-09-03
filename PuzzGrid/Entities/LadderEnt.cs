using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderEnt : GridEntity
{
    public override bool CanMove => false;
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set)
    {
        return null;
    }

    public override IEnumerable<GridActionSet> GetSettlementMoves(GridActionSummary actionSummary)
    {
        return null;
    }

    public override GridActionSetGroup GetGravityMoves()
    {
        return null;
    }

    public override GridActionConsequences GetConsequences(GridAction action)
    {
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
