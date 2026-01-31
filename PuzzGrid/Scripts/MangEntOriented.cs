#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class MangEntOriented : GridEntity, IPushesButton, IReceivesInput
{

    public bool InvertX;
    [SerializeField] private bool turn180 = false;

    public override GridActionSetGroup GetGravityMoves()
    {
        //return null;
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this,Vector3.down * 10,PushForce.WeakGravity, false, default));
    }


    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary)
    {

        GridActionSet result = null;
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
                            var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force, false,
                                transform.rotation);
                            if (result == null) result = newmove;
                            else result.Actions.AddRange(newmove.Actions);

                        }
                    }
                }
            }
        }

        return result;
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

    public  GridActionSet GetWalkMove(Vector3 dir)
    {
        var direction = InvertX ? new Vector3(-dir.x, dir.y, dir.z) : dir;
        
        var currentForward = this.transform.forward;

        if (GetComponent<SettlementGaze>()._partner == null)
        {
            var dot = turn180 ? direction.Dot(currentForward) : Math.Abs(direction.Dot(currentForward));
            if (dot < 0.9f) //not facing the right way!
            {
                var targetRot = Quaternion.LookRotation(direction, Vector3.up);
                return SimpleRotateAction.GetMove(
                    this,
                    targetRot,
                    this.transform.rotation);
            }
        }

        return SimpleMoveAction.GetMove(this, direction, GetWalkForce(), false, default);
        
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

        if (action is SimpleMoveAction move)
        {
            if (move.Force == PushForce.WeakGravity)
            {
                if (move.Ent.TryGetComponent<SettlementGaze>(out var gazer))
                {
                    if (gazer._partner != null)
                    {
                        Debug.Log(gazer._partner);
                        return new GridActionConsequences()
                        {
                            Dependency = new SimpleMoveAction(gazer._partner.Entity, move.MovementVec, move.Force),
                            Approval = Approvals.Partial
                        };
                    }

                    ;
                }
                
                
            }
        }
        
        return GridActionConsequences.ActionApproved; //unsure what to do as a default. i guess nothing.
    }
}

#endif
#endif
#endif