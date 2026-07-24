#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections.Generic;
using System.Linq;
using stoogebag.Extensions;
using UnityEngine;

public class BuoyantEnt : MonoBehaviour, IGridEntityComponent
{
    public GridActionSet GetSettlementMoves(GridActionSummary set)
    {
        return null;
    }

    public GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set)
    {
        if (set.Any(t => t.Ent == Entity)) return null;
        foreach (var gridAction in set)
        {
            
            if (gridAction.Ent.name.Contains("water"))
            {

                Physics.SyncTransforms();
                var box = gridAction.Ent.GetComponent<BoxCollider>();
                var myBox = gameObject.GetComponentInDescendants<BoxCollider>();
                
                if (!BoxOverlap(box, myBox)) continue;
                        
                
                if (OverlapMoreThanHalf( myBox, box))
                {
                    print("im so wet im being buoyant");
                    return SimpleMoveAction.GetMove(Entity, Vector3.up * 10f, PushForce.Water, false,
                        transform.rotation);
                    
                    
                }
            }
        }

        return null;
    }

    
    public static bool BoxOverlap(BoxCollider a, BoxCollider b)
    {
        return Physics.ComputePenetration(
            a, a.transform.position, a.transform.rotation,
            b, b.transform.position, b.transform.rotation,
            out _, out _);
    }
    public static bool OverlapMoreThanHalf(BoxCollider a, BoxCollider b, int gridDivisions = 5)
    {
        var halfA = a.size * 0.5f;
        int inside = 0, total = 0;
        for (int x = 0; x < gridDivisions; x++)
        for (int y = 0; y < gridDivisions; y++)
        for (int z = 0; z < gridDivisions; z++)
        {
            var localPt = new Vector3(
                -halfA.x + (x + 0.5f) * (a.size.x / gridDivisions),
                -halfA.y + (y + 0.5f) * (a.size.y / gridDivisions),
                -halfA.z + (z + 0.5f) * (a.size.z / gridDivisions)
            ) + a.center;
            var worldPt = a.transform.TransformPoint(localPt);
            var bLocal = b.transform.InverseTransformPoint(worldPt) - b.center;
            var halfB = b.size * 0.5f;
            if (Mathf.Abs(bLocal.x) <= halfB.x &&
                Mathf.Abs(bLocal.y) <= halfB.y &&
                Mathf.Abs(bLocal.z) <= halfB.z)
                inside++;
            total++;
        }
        return (float)inside / total > 0.5f;
    }
    
    public GridEntity Entity { get; set; }
}
#endif
#endif
#endif