// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using DG.Tweening.Core;
// using DG.Tweening.Plugins.Options;
// using stoogebag.Extensions;
// using UnityEngine;
//
// public class CompoundGrowAction : GridAction
// {
//     // public override IEnumerable<UniTask> GetExecutionTasks()
//     // {
//     //     
//     //     //assuming a speed of 100. assuming a move takes .1 seconds.
//     //     
//     //     var totalMovementVec = _successfulMoves
//     //         .SelectMany(t => t.Actions)
//     //         .OfType<SimpleMoveAction>()
//     //         .Aggregate(Vector3.zero, (current, move) => current + move.MovementVec);
//     //
//     //     var speed = 100f;
//     //     
//     //     //d = vt
//     //     //t = d/v
//     //     //v = d/t
//     //     //so moving 10 units in Xs is a speed of 10/X. solve for t
//     //     var totalTime = 10 / speed;
//     //     var time = totalMovementVec.magnitude / speed;
//     //
//     //     if (Ent is ElevatorEnt ee)
//     //     {
//     //         foreach (var task in ee.GetResizeTasks())
//     //         {
//     //             yield return task;
//     //         }
//     //     }
//     //     
//     //     
//     //     //consequences!
//     //
//     //     var consequenceGroups = _successfulMoves.Select(t => t.Consequences)
//     //         .WhereNotNull()
//     //         .SelectMany(t=>t.Actions)
//     //         .OfType<SimpleMoveAction>()
//     //         .GroupBy(t => t.Ent.ID);
//     //
//     //     foreach (var group in consequenceGroups)
//     //     {
//     //         var movement = group.Aggregate(Vector3.zero, (current, move) => current + move.MovementVec);
//     //         var moveTime = movement.magnitude / speed;
//     //         if (movement != Vector3.zero)
//     //         {
//     //             var ent = group.First().Ent;
//     //             
//     //             
//     //             
//     //             yield return CompoundMoveAction.DelayThenMoveTask(totalTime- moveTime, 
//     //                 ent.transform.DOMove(ent.transform.position + movement, moveTime)
//     //                     .SetEase(Ease.InOutSine)
//     //             );
//     //                 
//     //         }
//     //     }
//     // }
//
//
//     public override bool ConflictsWith(GridAction sideEffectAction) => false;
//     public override UniTask GetExecutionTask()
//     {
//         throw new NotImplementedException();
//     }
//
//     public override UniTask GetUndoTask()
//     {
//         throw new NotImplementedException();
//     }
//
//
//     public Vector3 GrowVec;
//
//     static int _idCounter = 0;
//
//     public CompoundGrowAction(GridEntity ent, Vector3 growVec, int numMoves, bool partialMove, bool moveBackOnFail)
//     {
//         Ent = ent;
//         GrowVec = growVec;
//         ID = _idCounter++; 
//         
//         NumMoves = numMoves;
//         PartialMove = partialMove;
//     }
//
//     private int NumMoves;
//     private bool PartialMove;
//
//     private List<GridActionSet> _successfulMoves = new List<GridActionSet>();
//
//     public int SuccessfulMovesCount => _successfulMoves.SelectMany(t => t.Actions).Count();
//     
//     public override bool Evaluate(ref HashSet<GridAction> evaluatedActions)
//     {
//         _successfulMoves.Clear();
//         var moveNum = 0;
//         
//         while (true)
//         {
//             //if (Ent is MangEnt mang)
//             {
//                 var move = SimpleGrowAction.GetMove(Ent,GrowVec);
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
//                 if(moveNum >= NumMoves)
//                     break;
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
//         //Execute();
//
//
//         //await NodeEnt.TweenToPosition();
//         //NodeEnt.SetExecuting(false);
//     }
//
//     public override void Undo()
//     {
//         // NodeEnt.CurrentNode.EntityExited(NodeEnt, this);
//         // NodeEnt.CurrentNode = Start;
//         // NodeEnt.CurrentNode.EntityEntered(NodeEnt, this);
//         // NodeEnt.SetExecuting(false);
//         
//         
//         foreach (var set in _successfulMoves)
//         {
//             set.Undo();
//         }
//     }
//
//
//
// //this should be IN ORDER of what node's consequence should 'happen first'. maybe
//     public override IEnumerable<GridNode> GetRelevantNodes()
//     {
//         yield break;
//         //  yield return Finish;
//         // // yield return Start.PuzzGrid.GetNodeAtPos((Start.Position + Finish.Position) / 2);
//         //  yield return Start;
//     }
//     
//     
//     protected bool Equals(SimpleMoveAction other)
//     {
//         return Equals(Ent, other.Ent) && GrowVec.Equals(other.MovementVec);
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
//         return HashCode.Combine(Ent, GrowVec);
//     }
//     
//     
//     // public static GridActionSet GetMove(GridEntity ent, Vector3 dir, int numMoves, bool partialMove)
//     // {
//     //     return new GridActionSet(ent.PuzzGrid)
//     //     {
//     //         Actions = new CompoundMoveAction(ent, dir, numMoves, partialMove, true).One().ToList<GridAction>(),
//     //     };
//     // }
//     
// }