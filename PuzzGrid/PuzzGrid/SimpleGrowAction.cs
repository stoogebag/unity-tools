// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using stoogebag.Extensions;
// using UnityEngine;
//
//
// public class SimpleGrowAction : GridAction, IPushAction
// {
//     public PushForce Force { get; } //this might need to change to a getter or something...
//
//     public Vector3 MovementVec { get; }
//
//     static int _idCounter = 0;
//
//     public SimpleGrowAction(GridEntity ent, Vector3 movementVec, bool isPlatformPush= false)
//     {
//         PlatformPush = isPlatformPush;
//         Ent = ent;
//         MovementVec = movementVec;
//         ID = _idCounter++; //todo: consider if this is a good idea. will they always be ordered by creation time?
//     }
//
//     public bool PlatformPush { get; set; }
//
//
//     public static GridActionSet GetMove(GridEntity ent, Vector3 dir)
//     {
//         return GridActionSet.GetSingle(ent.PuzzGrid, new SimpleGrowAction(ent, dir));
//         
//     }
//     
//     
//     public override bool Evaluate(ref HashSet<GridAction> evaluatedActions)
//     {
//         
//         //if (Ent is BlockEnt block)
//         {
//             foreach (var gridNode in Ent.GetNeighbours(MovementVec))
//             {
//                 foreach (var nodeEntity in gridNode.Entities)
//                 { 
//                     //this is perhaps inelegant. but we run 2 checks, first for summary 
//                     //rejection and then evaluating consequences if required. 
//                     var (pass, cons) = nodeEntity.GetConsequences(this);
//                     if (!pass) return false;
//                     
//                     if (cons?.Dependency == null) continue;
//                     if (!evaluatedActions.Add(cons.Dependency)) continue; //action is a duplicate
//                     var passed = cons.Dependency.Evaluate(ref evaluatedActions);
//                     if (!passed) return false;
//                 }
//             }
//         }
//
//         return true;
//     }
//
//     public override void Execute()
//     {
//         // foreach (var ent in Ent.NodeEnts)
//         // {
//         //     ent.CurrentNode.EntityExited(ent, this);
//         //     ent.CurrentNode = ent.CurrentNode.GetNeighbour(MovementVec);
//         //     //ent.transform.position = ent.CurrentNode.Position;
//         //     ent.CurrentNode.EntityEntered(ent, this);
//         // }
//
//         foreach (var neighbour in Ent.GetNeighbours(MovementVec))
//         {
//             var newEnt = Ent.CreateNodeEnt();
//             newEnt.GridEntity = Ent;
//             newEnt.CurrentNode = neighbour;
//             Ent.NodeEnts.Add(newEnt);
//             newEnt.CurrentNode.EntityEntered(newEnt, this);
//         }
//     }
//
//
//     public override void Undo()
//     {
//         foreach (var nodeEntity in Ent.GetFrontier(MovementVec))
//         {
//             nodeEntity.CurrentNode.EntityExited(nodeEntity, this);
//             Ent.NodeEnts.Remove(nodeEntity);
//         }
//     }
//
//     public override async UniTask ExecuteFinally()
//     {
//     }
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
//     public override bool ConflictsWith(GridAction sideEffectAction) => false;
// }