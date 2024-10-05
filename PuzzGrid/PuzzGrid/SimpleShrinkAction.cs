// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using stoogebag.Extensions;
// using UnityEngine;
//
//
// public class SimpleShrinkAction : GridAction
// {
//
//
//     public Vector3 DirectionVec { get; }
//
//     static int _idCounter = 0;
//
//     public SimpleShrinkAction(GridEntity ent, Vector3 directionVec, bool allowDisappear)
//     {
//         Ent = ent;
//         DirectionVec = directionVec;
//         ID = _idCounter++; //todo: consider if this is a good idea. will they always be ordered by creation time?
//         AllowDisappear = allowDisappear;
//     }
//
//     public bool AllowDisappear { get; set; }
//
//
//     public static GridActionSet GetMove(GridEntity ent, Vector3 dir, bool allowDisappear)
//     {
//         return GridActionSet.GetSingle(ent.PuzzGrid, new SimpleShrinkAction(ent, dir, allowDisappear));
//     }
//     
//     
//     public override bool Evaluate(ref HashSet<GridAction> evaluatedActions)
//     {
//         if (Ent.NodeEnts.Count == 0) return false;
//         if (AllowDisappear) return true;
//
//         var frontier = Ent.GetFrontier(DirectionVec);
//         if (frontier.Count == Ent.NodeEnts.Count) return false;
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
//         foreach (var nodeEntity in Ent.GetFrontier(DirectionVec))
//         {
//             nodeEntity.CurrentNode.EntityExited(nodeEntity, this);
//             Ent.NodeEnts.Remove(nodeEntity);
//         }
//     }
//
//
//     public override void Undo()
//     {
//         
//         foreach (var neighbour in Ent.GetNeighbours(DirectionVec))
//         {
//             var newEnt = Ent.CreateNodeEnt();
//             newEnt.GridEntity = Ent;
//             newEnt.CurrentNode = neighbour;
//             Ent.NodeEnts.Add(newEnt);
//             newEnt.CurrentNode.EntityEntered(newEnt, this);
//         }
//         
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
//         return Equals(Ent, other.Ent) && DirectionVec.Equals(other.MovementVec);
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
//         return HashCode.Combine(Ent, DirectionVec);
//     }
//     
//     
//     public override bool ConflictsWith(GridAction sideEffectAction) => false;
// }