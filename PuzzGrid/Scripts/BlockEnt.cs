#if UNITASK && ODIN_INSPECTOR && UNIRX
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
        
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this,Vector3.down * 100,PushForce.WeakGravity, false, default));
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
        
        //ice!

        foreach (var action in actionSummary.ExecutedMoveSummary)
        {
            if (action.Ent == this)
            {
                if (action is SimpleMoveAction move)
                {
                    var down = GetNeighbours(Vector3.down * 10);
                    if (down != null && down.Count() > 0)
                    {
                        //theres a grippy surface below us.
                        var nonIce = down.Any(t => t.HitEnt.gameObject.GetComponent<Ice>() == null);
                        if (!nonIce)
                        {
                            var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force, false, transform.rotation);
                            if (result == null) result = newmove;
                            else result.Actions.AddRange(newmove.Actions);
                            
                        }
                    }
                }
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
         for (var i = effects.Count - 1; i >= 0; i--)
         {
            var gridAction = effects[i];
            if (gridAction is SimpleMoveAction move)
            {
                //var newMove = new CompoundMoveAction(this, move.MovementVec, move.NumMoves, true, false, move.Force);
                var moveDistance = move.MovementVec.magnitude;
                foreach (var action in PendingMoves.OfType<SimpleMoveAction>())
                 {
                     if (action.MovementVec.IsInSameDirection( move.MovementVec))
                     {
                         //these are duplicates!
                         moveDistance -= action.MovementVec.magnitude;
                     }
                 }
                
                if (moveDistance == move.MovementVec.magnitude) continue; //nothing happened
                
                effects.RemoveAt(i);
                if(moveDistance <= 0.01f) continue; //move is totally cancelled
                var newMove = new SimpleMoveAction(this, move.MovementVec.WithMagnitude(moveDistance), move.Force, move.PlatformPush, move.Turn);
                effects.Insert(i, newMove); //move is replaced
            }
        }

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
#endif