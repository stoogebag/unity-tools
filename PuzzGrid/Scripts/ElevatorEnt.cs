using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

public class ElevatorEnt : GridEntity, IActivateable, IActivatesByParents
{
    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;

    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary)
    {
        var active = (Parents.All(t => t.Activated.Value));

        if (active != Activated.Value) //need to switch. 
        {
            return GridActionSet.GetSingle(PuzzGrid, new ActivationGridAction(this));
        }
        else return null;

    }

    public override GridActionSetGroup GetGravityMoves() => null;

    public override GridActionConsequences GetConsequences(GridAction action) => null;

    public GameObject GameObject => this.gameObject;
    public BoolReactiveProperty Activated { get; } = new BoolReactiveProperty();

    [SerializeReference] private List<GameObject> _parents;
    private List<IActivateable> Parents;

    private void OnValidate()
    {
        if (!_parents.All(t => t.GetComponent<IActivateable>() != null))
        {
            _parents.Clear();
        }
        Parents = _parents.Select(t => t.GetComponent<IActivateable>()).ToList();
    }
}

public interface IActivatesByParents
{
    //public List<IActivateable> Parents { get; } 
    //todo: figure out how best to deal with this and the inspector...
}