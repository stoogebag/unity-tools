#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class FloorEnt : GridEntity
{
    public override GridActionSetGroup GetGravityMoves()
    {
        return null;
    }



    public override IEnumerable<GridActionSet> GetSettlementMoves(GridActionSummary actionSummary)  {
        
        //fire propagation.
        if (gameObject.TryGetComponent<Fire>(out var fire))
        {
            var vecs = new []{ transform.forward, -transform.forward, transform.right, -transform.right, transform.up  };
            
            foreach (Vector3 v in vecs)
            {
                var hits = GetAllNeighbours(v)?
                    .Select(t => t.HitEnt?.GetComponent<Oil>())
                    .WhereNotNull()
                    .Where(t=>t.gameObject.activeInHierarchy)
                    .ToHashSet();

                
                
                if (hits?.Any() == true)
                {
                    var result = new GridActionSet(PuzzGrid);
                    var firePrefab = PuzzGrid.GetComponent<PrefabDirectory>().firePrefab;
                    foreach (var hit in hits)
                    {
                        var ent = hit.Entity;
                        var spawnFire = new SpawnEntityAction()
                        {
                            EntityPrefab = firePrefab,
                            Position = ent.transform.position,
                            Rotation = ent.transform.rotation,
                            Grid = PuzzGrid
                        };

                        var destroyOil = new DisableEntityAction()
                        {
                            Ent = hit.Entity
                        };

                        result.Actions.Add(spawnFire);
                        result.Actions.Add(destroyOil);
                    }

                    yield return result;
                }
            }
        }

    }
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;
    
    
    public override GridActionConsequences GetConsequences(GridAction action)
    {
        if (action is IPushAction move)
        {
            return GridActionConsequences.ActionFailed;
        }
        return GridActionConsequences.ActionApproved;
        
    }
}
#endif
#endif
#endif