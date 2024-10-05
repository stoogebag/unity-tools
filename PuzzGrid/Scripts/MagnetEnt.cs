using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetEnt : MonoBehaviour, IGridEntityComponent
{
    public GridEntity Entity { get; set; }

    public GridActionSet GetSettlementMoves(GridActionSetGroup set) => null;

    public GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set)
    {
        GridActionSet result = null;

        // foreach (var gridAction in set)
        // {
        //     if (gridAction is CompoundMoveAction move)
        //     {
        //         if (move.Ent.TryGetComponent<MagnetEnt>(out var mag))
        //         {
        //             if (mag.Entity == Entity) continue;
        //             var count = move.NumMoves;
        //             if (Entity.GetAllNeighbourEntities().Contains(mag.Entity))
        //             {
        //                 var cma = new CompoundMoveAction(Entity, move.MovementVec, count, true, false, move.Force);
        //                 GridActionSet.Include(Entity.PuzzGrid, cma, ref result);
        //             }
        //         }
        //     }
        // }

        return result;
    }
    
    
    


}

public interface IGridEntityComponent
{
    public GridActionSet GetSettlementMoves(GridActionSetGroup set);
    public GridActionSet GetSideEffectMoves(IEnumerable<GridAction> sets);

    GridEntity Entity { get; set; }
    
    public void BindGridEntity(GridEntity entity)
    {
        Entity = entity;
    }

}

