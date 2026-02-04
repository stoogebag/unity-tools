using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Oil : MonoBehaviour, IGridEntityComponent
{

    public GridActionSet GetSettlementMoves(GridActionSummary set)
    {
        var up = Entity.GetNeighbours(Vector3.up * 10);

        //todo: this is assuming there's only one guy above me at a time. might need to rethink that.
        //also, what if a big object is only partially on ice.
        //in fact yes. its the slider who should decide if he slides.
        return null;
        var pusher = up?.FirstOrDefault(t => t.HitEnt != null);

        // foreach (var action in set.ExecutedMoveSummary)
        // {
        //     if (action.Ent == pusher)
        //     {
        //         //if()
        //     }
        // }
    }

    public GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;
    public GridEntity Entity { get; set; }

}