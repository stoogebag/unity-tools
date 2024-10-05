// using System;
// using System.Collections.Generic;
// using System.Linq;
// using stoogebag.Extensions;
// using UnityEngine;
//
// public class GridNode
// {
//     
//     private static int _nodeID = 0;
//
//     
//     public GridNode(PuzzGrid puzzGrid, Vector3 position, int x, int y, int z)
//     {
//         this.Position = position;
//         X = x;
//         Y = y;
//         Z = z;
//         PuzzGrid = puzzGrid;
//         NodeID = _nodeID.ToString();
//         _nodeID++;
//     }
//
//     public PuzzGrid PuzzGrid;
//
//
//     public Vector3 Position;
//     //public List<NodeEntity> Entities { get; private set; } = new List<NodeEntity>();
//     //public List<int> Entities { get; private set; } = new List<int>();
//
//
//     public IEnumerable<NodeEntity> Entities
//     {
//         get
//         {
//             for (var i = 0; i < _nodeEntIDs.Length; i++)
//             {
//                 var id = _nodeEntIDs[i];
//                 if (id == 0) continue;
//                 
//                 yield return PuzzGrid.NodeEnts[id];
//             }
//         }
//     }
//     
//     private int[] _nodeEntIDs = new int[4];
//
//     
//     public int X { get; internal set; }
//     public int Y { get; internal set; }
//     public int Z { get; internal set; }
//
//     public string NodeID { get; }
//
//     public override string ToString()
//     {
//         return $"{X},{Y},{Z}";
//     }
//
//     public void EntityEntered(NodeEntity nodeEnt, GridAction action)
//     {
//   //      if(_ents == null)
//  //           _ents = new bool[PuzzGrid.NodeEnts.Length];
// //        if (action is MoveNodeAction mna)
//         {
//             
//             for (var i = 0; i < _nodeEntIDs.Length; i++)
//             {
//                 if (_nodeEntIDs[i] == 0)
//                 {
//                     _nodeEntIDs[i] = nodeEnt.ID;
//                     return;
//                 }
//             }
//             //Entities.Add(nodeEnt);
//      //       _ents[nodeEnt.ID] = true;
//             //EntityEntered(nodeEnt, dir);
//             return;
//         }
//     }
//     public void EntityExited(NodeEntity nodeEnt, GridAction action)
//     {
//         for (var i = 0; i < _nodeEntIDs.Length; i++)
//         {
//             if (_nodeEntIDs[i] == nodeEnt.ID)
//             {
//                 _nodeEntIDs[i] = 0;
//                 break;
//                 
//             }
//         }
//
//
//         //_ents[nodeEnt.ID] = false;
//         //Entities.Remove(nodeEnt);
//     }
//
//     // public GridActionSetGroup GetSettlementMoves(GridActionSetGroup set)
//     // {
//     //     return new GridActionSetGroup(PuzzGrid)
//     //     {
//     //         ActionSets = Entities.Select(t => t.GetSettlementMoves(set))
//     //             .Where(t => t != null)
//     //             .ToList(),
//     //     };
//     // }
//
//
//     public GridNode GetNeighbour(Direction dir)
//     {
//         var n = neighbours[(int)dir];
//         if (n == null) n = neighbours[(int)dir] = ComputeNeighbour(dir);
//         return n;
//     }
//     
//     public GridNode ComputeNeighbour(Direction dir)
//     {
//         //handle portals!
//         
//         
//         if (dir == Direction.left) return PuzzGrid.GetNodeAtPos(X - 1, Y, Z);
//         if (dir == Direction.right) return PuzzGrid.GetNodeAtPos(X + 1, Y, Z);
//         if (dir == Direction.up) return PuzzGrid.GetNodeAtPos(X, Y + 1, Z);
//         if (dir == Direction.down) return PuzzGrid.GetNodeAtPos(X, Y - 1, Z);
//         if (dir == Direction.north) return PuzzGrid.GetNodeAtPos(X, Y, Z + 1);
//         if (dir == Direction.south) return PuzzGrid.GetNodeAtPos(X, Y, Z - 1);
//
//         throw new Exception("invalid direction.!");
//         return null;
//     }
//
//     public Vector3 GetPosition(NodeEntity nodeEnt)
//     {
//         return Position;
//     }
//
//     private GridNode[] neighbours = new GridNode[6];
//
// }
//

namespace stoogebag.PuzzGrid
{


    public enum Direction
    {
        up,
        down,
        left,
        right,
        north,
        south,
    }
}