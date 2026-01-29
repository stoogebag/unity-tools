#if UNITASK && ODIN_INSPECTOR && UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;


public class SimpleRotateAction : GridAction
{

    public Quaternion Orientation;
    public Quaternion OriginalOrientation;
    
    static int _idCounter = 0;
    private bool _aborted;

    public SimpleRotateAction(GridEntity ent, Quaternion newOrientation,
       Quaternion originalOrientation = default)
    {
        Orientation = newOrientation;
        Ent = ent;
        
        ID = _idCounter++; //todo: consider if this is a good idea. will they always be ordered by creation time?
        OriginalOrientation = originalOrientation;
    }

    public bool Turn { get; set; }



    public static GridActionSet GetMove(GridEntity ent, Quaternion orientation, Quaternion originalOrientation)
    {
        return new GridActionSet(ent.PuzzGrid)
        {
            Actions = new SimpleRotateAction(ent, 
                orientation, originalOrientation).One().ToList<GridAction>(),
        };
    }

    List<SimpleMoveAction> _approvedMovements;

    //evaluated actions prevents us from moving something twice in one actionset...
     public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
     {
         Evaluated = true;
         return this;
     }



    //i think not used. if it is, need to decide how. add the angles? idk. shouldnt matter in practice i guess.
    public SimpleRotateAction Merge(IEnumerable<SimpleRotateAction> actions)
    {
        var vec = Vector3.zero;

        if (vec == Vector3.zero) return null;
        var newMove = new SimpleRotateAction(Ent, Orientation, OriginalOrientation);
        return newMove;
    }


    public SimpleRotateAction Merge(SimpleRotateAction a1, SimpleRotateAction a2)
    {
        return Merge(a1.ToEnumerable(a2));
    }

    public override void Execute()
    {
        Ent.PendingMoves.Add(this);

        Ent.transform.rotation = this.Orientation;
    }


    public override void Undo()
    {
        if (_aborted) return;
        Ent.PendingMoves.Remove(this);
        Ent.transform.rotation = OriginalOrientation;
        
    }


    protected bool Equals(SimpleRotateAction other)
    {
        return Equals(Ent, other.Ent) && Orientation.Equals(other.Orientation);
    }


    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SimpleMoveAction)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ent, Orientation);
    }

    public override bool ConflictsWith(GridAction sideEffectAction) => false;

    public async override UniTask GetExecutionTask()
    {
        
        Ent.transform.rotation = this.Orientation;
       // Ent.transform.position -= MovementVec;
       // await Ent.transform.DOMove(Ent.transform.position+MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask();
    }
    
    public async override UniTask GetUndoTask()
    {
        
        Ent.transform.rotation = this.OriginalOrientation;
        // Ent.transform.position += MovementVec;
        // await Ent.transform.DOMove(Ent.transform.position-MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask();
    }

}


#endif