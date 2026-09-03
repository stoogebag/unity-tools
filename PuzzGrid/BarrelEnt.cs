#if UNITASK && ODIN_INSPECTOR && UNIRX
using System.Collections.Generic;
using System.Linq;
using stoogebag.Extensions;
using UnityEngine;

public class BarrelEnt : GridEntity, IPushesButton
{

    public override bool CanMove => true;
    public override GridActionSetGroup GetGravityMoves()
    {
        //return null;
        
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this,Vector3.down * 100,PushForce.WeakGravity));
    }
    

    public override IEnumerable<GridActionSet> GetSettlementMoves(GridActionSummary actionSummary) {
     
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
        
        foreach (var action in actionSummary.ExecutedMoveSummary)
        {
            if (action.Ent == this)
            {
                if (action is SimpleMoveAction move)
                {
                    if (IsRoll(move.MovementVec))
                    {
           //             var newmove = RollMoveAction.GetMove(this, move.MovementVec, PushForce.WeakSlide, false, transform.rotation);
           //             return newmove;
                    }
                }
            }
        }
        
        
        //ice! todo: move this to a component like 'SlideSettlementProvider'
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
                            var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force);
                            if (result == null) result = newmove;
                            else result.Actions.AddRange(newmove.Actions);
                            yield return result;
                        }
                    }
                }
            }
        }

        
    }

    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> sets)
    {
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

    
    private bool IsRoll(Vector3 direction)
    {
        if (direction.IsParallel(Vector3.up)) return false;
        if (direction.IsParallel(transform.up)) return false;
        return true;
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

            if (action is SimpleMoveAction moveAction)
            {
                if (IsRoll(moveAction.MovementVec))
                {
                    
                }
                else
                {
                    return 
                        new GridActionConsequences() { Dependency = new SimpleMoveAction(this, push.MovementVec, push.Force) , Approval = Approvals.Partial};    
                }
            }
        }

        return GridActionConsequences.ActionApproved;
    }
}
#endif