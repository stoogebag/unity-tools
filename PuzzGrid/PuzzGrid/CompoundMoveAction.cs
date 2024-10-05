 using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using stoogebag.Extensions;
using UnityEngine;
//
// public class CompoundMoveAction : GridAction
// {
//
//     public PushForce Force;
//
//     public override IEnumerable<GridAction> GetApprovalSummary(bool approved)
//     {
//         var allActionSets = _successfulMoves.GetAllDescendants(t => t.Consequences?.One());
//         var allActions = allActionSets.SelectMany(t => t.Actions).OfType<SimpleMoveAction>();
//         //_successfulMoves.Select(t=>t.Consequences).WhereNotNull().ForEach(t=>allActions.AddRange(t.Actions));
//         
//         foreach (var group in allActions.WhereNotNull().OfType<SimpleMoveAction>().GroupBy(t=>t.Ent))
//         {
//             var ent = group.Key;
//             var moveCount = group.Count();
//             var vec = group.First().MovementVec;
//             
//             yield return new CompoundMoveAction(ent, vec, moveCount, PartialMove, MoveBackOnFail, Force);
//         }
//     }
//
//     public override IEnumerable<GridAction> GetFailureSummary(bool approved)
//     {
//         if(approved) yield break;
//
//         if (Force.IsGravity) yield break; //hacky? might need to make this more flexible...
//         if (_successfulMoves.Count == NumMoves) yield break;
//
//         yield return new CompoundMoveAction(Ent, MovementVec, NumMoves- _successfulMoves.Count, PartialMove, MoveBackOnFail, Force);
//         yield break;
//         
//         //todo:handle the multis.
//         var allActionSets = _successfulMoves.GetAllDescendants(t => t.Consequences?.One());
//         var allActions = allActionSets.SelectMany(t => t.Actions).OfType<SimpleMoveAction>();
//         //_successfulMoves.Select(t=>t.Consequences).WhereNotNull().ForEach(t=>allActions.AddRange(t.Actions));
//
//         foreach (var group in allActions.WhereNotNull().OfType<SimpleMoveAction>().GroupBy(t=>t.Ent))
//         {
//             var ent = group.Key;
//             var moveCount = group.Count();
//             var vec = group.First().MovementVec;
//             
//             yield return new CompoundMoveAction(ent, vec, moveCount, PartialMove, MoveBackOnFail, Force);
//         }
//     }
//
//
//     public static UniTask DelayThenMoveTask(float delay, TweenerCore<Vector3, Vector3, VectorOptions> tween)
//     {
//         var tweenSeq = DOTween.Sequence();
//         tweenSeq.AppendInterval(delay);
//         
//         tweenSeq.Append(tween);
//         return tweenSeq.ToUniTask();
//     }
//
//     public Vector3 MovementVec;
//
//     static int _idCounter = 0;
//
//     public CompoundMoveAction(GridEntity ent, Vector3 movementVec, int numMoves, bool partialMove, bool moveBackOnFail, PushForce force)
//     {
//         Force = force;
//         Ent = ent;
//         MovementVec = movementVec;
//         ID = _idCounter++; 
//         
//         NumMoves = numMoves;
//         MoveBackOnFail = moveBackOnFail;
//         PartialMove = partialMove;
//     }
//
//     public int NumMoves;
//     private bool MoveBackOnFail;
//     private bool PartialMove;
//
//     private List<GridActionSet> _successfulMoves = new List<GridActionSet>();
//
//     public Vector3 GetTotalMovementVec()
//     {
//         return _successfulMoves
//             .SelectMany(t => t.Actions)
//             .Cast<SimpleMoveAction>()
//             .Aggregate(Vector3.zero, (current, move) => current + move.MovementVec);
//     }
//
//     public int SuccessfulMovesCount => _successfulMoves.SelectMany(t => t.Actions).Count();
//     
//     public override bool Evaluate(ref HashSet<GridAction> evaluatedActions)
//     {
//         _successfulMoves.Clear();
//         var moveNum = 0;
//
//         if (Force.IsGrounded && !Force.Grounding.Any())
//         {
//             Evaluated = true;
//             return false;
//         }
//         
//         //fail for magnetism and gravity.
//         //todo: MAKE IT WORK
//         if (Force.IsGravity)
//         {
//             if (Ent.gameObject.TryGetComponent<MagnetEnt>(out var magnet))
//             {
//                 foreach (var node in Ent.GetAllNeighbours())
//                 {
//                     foreach (var nodeEntity in node.Entities)
//                     {
//                         if (nodeEntity.GridEntity == Ent) continue;
//
//                         if (nodeEntity.GridEntity.TryGetComponent<MagnetEnt>(out var otherMagnet))
//                         {
//                             var (pass, cons) = nodeEntity.GetConsequences(this);
//                             if (!pass) return false;
//
//                             if (cons?.Dependency == null) continue;
//                             if (cons.Dependency.Equals(this)) continue;
//                             if (!evaluatedActions.Add(cons.Dependency)) continue; //action is a duplicate
//                             var passed = cons.Dependency.Evaluate(ref evaluatedActions);
//                             if (!passed) return false;
//                         }
//                     }
//                 }
//             }
//         }
//         
//         while (true)
//         {
//             //if (Ent is MangEnt mang)
//             {
//                 if(moveNum >= NumMoves)
//                     break;
//                 var move = SimpleMoveAction.GetMove(Ent,MovementVec, Force);
//                 
//                 var (success, action) = move.Evaluate();
//                 if (!success)
//                 {
//                     //we have failed.
//                     break;
//                 }
//                 
//                 _successfulMoves.Add(move);
//                 move.Execute();
//                 moveNum++;
//             }
//             
//         }
//         
//         var succeeded  =(moveNum == NumMoves) || (PartialMove && moveNum > 0);
//
//         //if (!succeeded)
//         {
//             for (var i = _successfulMoves.Count - 1; i >= 0; i--)
//             {
//                 _successfulMoves[i].Undo();
//             }
//         }
//
//         return (moveNum == NumMoves) || (PartialMove && moveNum > 0);
//     }
//
//     public override void Execute()
//     {
//         foreach (var set in _successfulMoves)
//         {
//             set.Execute();
//         }
//     }
//
//     public override async UniTask ExecuteFinally()
//     {
//         //todo:i think it can be removed.
//     }
//
//     public override void Undo()
//     {
//         foreach (var set in _successfulMoves)
//         {
//             set.Undo();
//         }
//     }
//
//
//     
//     
//     protected bool Equals(SimpleMoveAction other)
//     {
//         return Equals(Ent, other.Ent) && MovementVec.Equals(other.MovementVec);
//     }
//
//
//     public override bool Equals(object obj)
//     {
//         if (ReferenceEquals(null, obj)) return false;
//         if (ReferenceEquals(this, obj)) return true;
//         if (obj.GetType() != this.GetType()) return false;
//         return Equals((SimpleMoveAction)obj);
//     }
//
//     public override int GetHashCode()
//     {
//         return HashCode.Combine(Ent, MovementVec);
//     }
//     
//     
//     public static GridActionSet GetMove(GridEntity ent, Vector3 dir, int numMoves, bool partialMove, PushForce force)
//     {
//         return new GridActionSet(ent.PuzzGrid)
//         {
//             Actions = new CompoundMoveAction(ent, dir, numMoves, partialMove, true, force).One().ToList<GridAction>(),
//         };
//     }
//
//
//     public override bool ConflictsWith(GridAction sideEffectAction)
//     {
//         if (sideEffectAction is SimpleMoveAction move)
//         {
//             if (move.Ent == Ent) return true;
//         }
//         if (sideEffectAction is CompoundMoveAction cma)
//         {
//             if (cma._successfulMoves.Any(t => t.Consequences?.Actions.Any(x => x.Ent == Ent) == true)) return true;
//         }
//
//         
//         
//         return false;
//     }
//
//     public override UniTask GetExecutionTask()
//     {
//         //var movementSize = Ent.PuzzGrid.SizeX;
//         var movementSize = Ent.PuzzGrid.SpacingX;
//         return Ent.transform.DOMove(Ent.transform.position +  movementSize*MovementVec*NumMoves, 0.1f).ToUniTask();
//     }
//     public async override UniTask GetFailureTask()
//     {
//         var pos = Ent.transform.position;
//         await Ent.transform.DOMove(pos + MovementVec*.5f, 0.05f).ToUniTask();
//         await Ent.transform.DOMove(pos, 0.05f).ToUniTask();
//         
//     }
//     public override UniTask GetUndoTask()
//     {
//         var movementSize = Ent.PuzzGrid.SpacingX;
//         return Ent.transform.DOMove(Ent.transform.position - movementSize*MovementVec*NumMoves, 0.1f).ToUniTask();
//     }
//
//     
// }

public class PushForce
{
    public ForceStrength Strength;

    public bool IsGravity;
    public bool IsGrounded;
    public List<GridEntity> Grounding;
    
    public static PushForce WeakGravity = new PushForce()
    {
        Strength = ForceStrength.Weak,
        IsGravity = true,
        IsGrounded = false,
    };
    
    public static PushForce StrongGravity = new PushForce()
    {
        Strength = ForceStrength.Strong,
        IsGravity = true,
        IsGrounded = false,
    };
}

public enum ForceStrength
{
    Weak,
    Strong,
}