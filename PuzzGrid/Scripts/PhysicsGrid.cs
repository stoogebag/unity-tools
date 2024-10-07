using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

public class PhysicsGrid : MonoBehaviour
{
    public ActionQueue MoveQueue { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        MoveQueue = GetComponent<ActionQueue>();

        var puzzGrid = GetComponent<PuzzGrid>();
        puzzGrid.OnMoveFinishedObservable()
            .Merge(puzzGrid.OnUndoFinishedObservable()).Subscribe(u =>
            {
                CheckPortals();
            });
    }


    private void CheckPortals()
    {
        foreach (var physicsEnt in GetComponentsInChildren<PhysicsEnt>())
        {
            if (physicsEnt.IsClone) continue;
            //check if portals need to be portaled
            physicsEnt.CheckPortals();
        }
    }
}