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

public class MangEnt : GridEntity, IPushesButton, IReceivesInput
{
    public bool InvertX;

    public override bool CanMove => true;
    public bool turnOnMove;

    public override GridActionSetGroup GetGravityMoves()
    {
        if (!Gravity) return null;
        return GridActionSetGroup.GetSingle(SimpleMoveAction.GetMove(this, Vector3.down * 10, PushForce.WeakGravity,
            false, default));
    }


    public override IEnumerable<GridActionSet> GetSettlementMoves(GridActionSummary actionSummary)
    {
        if (actionSummary != null)
        {
            foreach (var action in actionSummary.ExecutedMoveSummary)
            {
                if (action.Ent == this)
                {
                    if (action is SimpleMoveAction sma)
                    {
                        if (action == actionSummary.ExecutedMoveSummary.Last())
                        {
                            for (var i = 0; i < actionSummary.ExecutedMoveSummary.Count; i++)
                            {
                                print(i + " move " + actionSummary.ExecutedMoveSummary[i].ToString());
                            }

                            if (sma.MovementVec.IsInSameDirection(Vector3.up))
                            {
                                if (sma.Force == PushForce.Climb)
                                {
                                    yield return SimpleMoveAction.GetMove(this, transform.forward * 10,
                                        PushForce.WeakSlide,
                                        true,
                                        transform.rotation);
                                }
                            }
                        }
                    }

                    if (GetComponent<Oil>() == null) //non-oily feet.
                    {
                        //sliipp
                        if (action is SimpleMoveAction move)
                        {
                            var down = GetAllNeighbours(Vector3.down * 10);
                            if (down != null && down.Count() > 0)
                            {
                                //theres a grippy surface below us.

                                //todo: large blocks with a nonslip area.
                                //will need to do some kind of GroupBy location and beware of snappage.

                                var slipperyTypes = new[] { typeof(Ice), typeof(Oil) };

                                Func<PuzzGridRaycastResult, bool> isSlippery = (r) =>
                                {
                                    foreach (var t in slipperyTypes)
                                    {
                                        if (r.HitEnt.GetComponent(t) != null) return true;
                                    }

                                    return false;
                                };
                                Func<PuzzGridRaycastResult, bool> isOily = (r) =>
                                {
                                    {
                                        if (r.HitEnt.GetComponent<Oil>() != null) return true;
                                    }
                                    return false;
                                };

                                var slip = down.Any(t => isSlippery(t));
                                if (slip)
                                {
                                    GridActionSet result = null;
                                    result = new GridActionSet(PuzzGrid);
                                    var oily = down.Any(t => isOily(t));
                                    if (oily)
                                    {
                                        var addOil = new AddComponentAction<Oil>()
                                        {
                                            Ent = this
                                        };
                                        result.Actions.Add(addOil);
                                        yield return result;
                                    }

                                    var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force, false,
                                        transform.rotation);

                                    yield return newmove;
                                }
                            }
                        }
                    }
                    else
                    {
                        {
                            var result = new GridActionSet(PuzzGrid);

                            var down = GetAllNeighbours(Vector3.down * 10)?.Select(t => t?.HitEnt).WhereNotNull();


                            if (down != null && down.Any() &&
                                down.All(t => t.GetComponent<Oil>() == null)) //no oil. add oil
                            {
                                var oilPrefab = PuzzGrid.GetComponent<PrefabDirectory>().oilPrefab;

                                var spawnOil = new SpawnEntityAction()
                                {
                                    EntityPrefab = oilPrefab,
                                    Position = this.transform.position + Vector3.down * PuzzGrid.GridSpacing(),
                                    Rotation = Quaternion.identity,
                                    Grid = PuzzGrid
                                };

                                result.Actions.Add(spawnOil);

                                yield return new GridActionSet(PuzzGrid)
                                {
                                    Actions = spawnOil.One().ToList<GridAction>()
                                };
                            }


                            if (action is SimpleMoveAction move)
                            {
                                var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force, false,
                                    transform.rotation);
                                yield return newmove;
                            }


                            //
                            //
                            // if (down != null && down.Count() > 0)
                            // {
                            //     //todo: large blocks with a nonslip area.
                            //     //will need to do some kind of GroupBy location and beware of snappage.
                            //
                            //     var slipperyTypes = new[] { typeof(Ice), typeof(Oil) };
                            //
                            //     Func<PuzzGridRaycastResult, bool> isSlippery = (r) =>
                            //     {
                            //         foreach (var t in slipperyTypes)
                            //         {
                            //             if (r.HitEnt.GetComponent(t) != null) return true;
                            //         }
                            //
                            //         return false;
                            //     };
                            //     Func<PuzzGridRaycastResult, bool> isOily = (r) =>
                            //     {
                            //         {
                            //             if (r.HitEnt.GetComponent<Oil>() != null) return true;
                            //         }
                            //         return false;
                            //     };
                            //
                            //     var slip = down.Any(t => isSlippery(t));
                            //     if (slip)
                            //     {
                            //         GridActionSet result = null;
                            //         result = new GridActionSet(PuzzGrid);
                            //         var oily = down.Any(t => isOily(t));
                            //         if (oily)
                            //         {
                            //             var addOil = new AddComponentAction<Oil>()
                            //             {
                            //                 Ent = this
                            //             };
                            //             result.Actions.Add(addOil);
                            //         }
                            //
                            //         var newmove = SimpleMoveAction.GetMove(this, move.MovementVec, move.Force, false,
                            //             transform.rotation);
                            //         result.Actions.AddRange(newmove.Actions);
                            //         return result;
                            //     }
                            // }
                        }
                    }
                }
            }
        }
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

    public GridActionSet GetWalkMove(Vector3 dir)
    {
        var direction = InvertX ? new Vector3(-dir.x, dir.y, dir.z) : dir;
        return SimpleMoveAction.GetMove(this, direction, GetWalkForce(), turnOnMove, default);
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
            return new GridActionConsequences()
                { Dependency = new SimpleMoveAction(this, push.MovementVec, push.Force), Approval = Approvals.Partial };
        }

        return GridActionConsequences.ActionApproved; //unsure what to do as a default. i guess nothing.
    }
}

#endif
#endif
#endif