using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class MangEnt : GridEntity, IPushesButton
{

    public bool InvertX;
    
    public override GridActionSetGroup GetGravityMoves()
    {
        //return null;
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this,Vector3.down * 10,PushForce.WeakGravity));
    }


    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary)
    {
        return null;
    }
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set)
    {
        GridActionSet result = null;
        
        foreach (var gridEntityComponent in _components)
        {
            var se = gridEntityComponent.GetSideEffectMoves(set);
            if (se != null)
            {
                if (result == null) result = se;
                else result.Actions.AddRange(se.Actions);
            }
        }

        return result;
    }

    

    

    public override IEnumerable<GridAction> FilterSideEffects(List<GridAction> effects)
    {
        var list = new List<GridAction>();
        
        // for (var i = effects.Count - 1; i >= 0; i--)
        // {
        //     var gridAction = effects[i];
        //     if (gridAction is CompoundMoveAction move)
        //     {
        //         var newMove = new CompoundMoveAction(this, move.MovementVec, move.NumMoves, true, false, move.Force);
        //         var numMoves = move.NumMoves;
        //         foreach (var action in PendingMoves.OfType<SimpleMoveAction>())
        //         {
        //             if (action.MovementVec == move.MovementVec)
        //             {
        //                 //these are duplicates!
        //                 numMoves--;
        //             }
        //         }
        //
        //         if (numMoves == move.NumMoves) continue; //nothing happened
        //         
        //         effects.RemoveAt(i);
        //         if(numMoves <= 0) continue; //move is totally cancelled
        //         
        //         newMove.NumMoves = numMoves;
        //         effects.Insert(i, newMove); //move is replaced
        //     }
        // }

        return effects;
    }

    public  GridActionSet GetWalkMove(Vector3 dir)
    {
        var direction = InvertX ? new Vector3(-dir.x, dir.y, dir.z) : dir;
        
        
        return SimpleMoveAction.GetMove(this, direction, GetWalkForce());
        return null;
        
    }

    private PushForce GetWalkForce()
    {
        var force = new PushForce()
        {
            IsGravity = false,
            Strength = ForceStrength.Strong,
            IsGrounded = true,
            //Grounding = GetAllNeighbourEntities(Direction.down).ToList(),
            Grounding = new List<GridEntity>(),
        };

        return force;
    }
    
    
    public override GridActionConsequences GetConsequences(GridAction action)
    {
        if (action is IPushAction push)
        {
            return new GridActionConsequences() { Dependency = new SimpleMoveAction(this, push.MovementVec, push.Force) , Approval = Approvals.Partial};
        }
        return GridActionConsequences.ActionApproved; //unsure what to do as a default. i guess nothing.
    }
}
