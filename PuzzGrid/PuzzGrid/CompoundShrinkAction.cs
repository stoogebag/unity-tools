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
// public class CompoundShrinkAction : GridAction
// {
//     
//
//     public Vector3 GrowVec;
//
//     static int _idCounter = 0;
//
//     public CompoundShrinkAction(GridEntity ent, Vector3 growVec, int numMoves, bool allowPartial = false,bool allowDisappear = false)
//     {
//         AllowDisappear = false; //todo: there's an issue where we would lose track of our location if we allowed disappear...
//         AllowPartial = allowPartial;
//         
//         Ent = ent;
//         GrowVec = growVec;
//         ID = _idCounter++; 
//         
//         NumMoves = numMoves;
//     }
//
//     private int NumMoves;
//     private bool AllowDisappear;
//     private bool AllowPartial;
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
//                 var move = SimpleShrinkAction.GetMove(Ent,GrowVec, AllowDisappear);
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
//
//         //if (!succeeded)
//         {
//             for (var i = _successfulMoves.Count - 1; i >= 0; i--)
//             {
//                 _successfulMoves[i].Undo();
//             }
//         }
//
//         return (moveNum == NumMoves) || (AllowPartial && moveNum > 0);
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
//     public override bool ConflictsWith(GridAction sideEffectAction) => false;
// }