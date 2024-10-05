using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

[Serializable]
public class BlockEnt : GridEntity, IPushesButton
{

    public override GridActionSetGroup GetGravityMoves()
    {
        //return null;
        
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this,Vector3.down * 100,PushForce.WeakGravity));
    }
    

    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary) {
     
        GridActionSet result = null;
        
        foreach (var gridEntityComponent in _components)
        {
            var se = gridEntityComponent.GetSettlementMoves(actionSummary);
            if (se != null)
            {
                if (result == null) result = se;
                else result.Actions.AddRange(se.Actions);
            }
        }

        return result;
        
    }

    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> sets)
    {
        //get all downstairs neighbours.

        GridActionSet result = null;
        
        foreach (var gridEntityComponent in _components)
        {
            var se = gridEntityComponent.GetSideEffectMoves(sets);
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
        //         
        //         //todo if needed: this could ez be optimised in the replacement case by jujst replacing the move. 
        //     }
        // }

        return effects;
    }
    
    
    public override GridActionConsequences GetConsequences(GridAction action)
    {
        if (action is IPushAction push)
        {
            if (push.Force.IsGrounded)
            {
               // if (push.Force.Grounding.All(t => t == this)) return (false, null);
               
               //TODO: reimplement maybe. for now grounding is empty.
            }
            
            //if (push.Entity != GridEntity)
            {
                return 
                    new GridActionConsequences() { Dependency = new SimpleMoveAction(this, push.MovementVec, push.Force) , Approval = Approvals.Partial};
            }

            //if(!move.PlatformPush)
            //    return (true, new GridActionConsequences() { Dependency = new SimpleMoveAction(GridEntity, move.MovementVec) });
            // else
            // {
            //     var neighbours = (GridEntity as BlockEnt).GetNeighbours(Vector3.down);
            //     
            //     foreach (var gridNode in neighbours)
            //     {
            //         if(gridNode.Entities.Any(t => t.GridEntity != move.Ent)) return (false, null);
            //     }
            //     
            //     return (true, new GridActionConsequences() { Dependency = new SimpleMoveAction(GridEntity, move.MovementVec) });
            // }
        }

        return GridActionConsequences.ActionApproved;
    }
}