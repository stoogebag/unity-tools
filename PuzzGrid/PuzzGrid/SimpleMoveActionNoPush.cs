#if UNITASK && ODIN_INSPECTOR && UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;


public class SimpleMoveActionNoPush : GridAction
{
    
        public PushForce Force { get; }
    public Vector3 MovementVec { get; }

    public Quaternion OriginalOrientation;

    static int _idCounter = 0;
    private bool _aborted;

    public SimpleMoveActionNoPush(GridEntity ent, Vector3 movementVec, PushForce force, bool isPlatformPush = false,
        bool turn = false, Quaternion originalOrientation = default)
    {
        PlatformPush = isPlatformPush;
        Force = force;
        Ent = ent;
        MovementVec = movementVec;
        ID = _idCounter++; //todo: consider if this is a good idea. will they always be ordered by creation time?
        Turn = turn;
        OriginalOrientation = originalOrientation;
    }

    public bool Turn { get; set; }

    public bool PlatformPush { get; set; }


    public static GridActionSet GetMove(GridEntity ent, Vector3 dir, PushForce force, bool turn,
        Quaternion originalOrientation)
    {
        return new GridActionSet(ent.PuzzGrid)
        {
            Actions = new SimpleMoveActionNoPush(ent, ent.PuzzGrid.GetDirectionVector(dir), force, turn: turn,
                originalOrientation: originalOrientation).One().ToList<GridAction>(),
        };
    }

    //evaluated actions prevents us from moving something twice in one actionset...
    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;
        return this;
    }



    public override void Execute()
    {
        // if (Ent.NodeEnts.First().CurrentNode != StartNode)
        // {
        //     //abort!
        //     _aborted = true;
        //     return;
        // }

        OriginalOrientation = Ent.transform.rotation;
        
        var dir = GridEntity.GetDirection(MovementVec);
        Ent.PendingMoves.Add(this);
        //var physicsEnt = Ent.GetComponent<PhysicsEnt>();

        // if (physicsEnt != null)
        // {
        //     physicsEnt.Moved = true;
        //     if (physicsEnt.IsClone)
        //     {
        //     }
        // }

        if (Turn)
        {
            Ent.transform.LookAt(Ent.transform.position + MovementVec, Vector3.up);
        }

        Ent.transform.position += MovementVec;


    }


    public override void Undo()
    {
        if (_aborted) return;
        Ent.PendingMoves.Remove(this);
        var dir = GridEntity.GetDirection(-MovementVec);

        Ent.transform.position -= MovementVec;

        if (Turn) { Ent.transform.rotation = OriginalOrientation; }
    }


    protected bool Equals(SimpleMoveAction other)
    {
        return Equals(Ent, other.Ent) && MovementVec.Equals(other.MovementVec);
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
        return HashCode.Combine(Ent, MovementVec);
    }

    public override string ToString()
    {
        return "SimpleMoveActionNoPush: " + Ent.name + " move " + MovementVec;
    }

    public override bool ConflictsWith(GridAction sideEffectAction) => false;

    public async override UniTask GetExecutionTask()
    {
        Ent.transform.position -= MovementVec;

        var tasks = new List<UniTask>();
        
        if (Turn)
        {
            Ent.transform.rotation = OriginalOrientation;
            tasks.Add(Ent.transform.DOLookAt(Ent.transform.position + MovementVec,  0.1f).SetEase(Ease.InOutSine).ToUniTask());
        }
        
        tasks.Add(Ent.transform.DOMove(Ent.transform.position + MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask());
        
        await UniTask.WhenAll(tasks);
    }

    public async override UniTask GetUndoTask()
    {
        Ent.transform.position += MovementVec;
        await Ent.transform.DOMove(Ent.transform.position - MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask();
    }
}
#endif