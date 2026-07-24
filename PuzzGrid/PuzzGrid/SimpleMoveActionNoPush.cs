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

    static int _idCounter = 0;
    private bool _aborted;

    private TurnData _turn;
    public TurnData Turn { get => _turn; set => _turn = value; }

    public SimpleMoveActionNoPush(GridEntity ent, Vector3 movementVec, PushForce force, bool isPlatformPush = false,
        TurnData turn = default)
    {
        PlatformPush = isPlatformPush;
        Force = force;
        Ent = ent;
        MovementVec = movementVec;
        ID = _idCounter++;
        _turn = turn;
    }

    public bool PlatformPush { get; set; }


    public static GridActionSet GetMove(GridEntity ent, Vector3 dir, PushForce force,
        TurnData turn = default)
    {
        return new GridActionSet(ent.PuzzGrid)
        {
            Actions = new SimpleMoveActionNoPush(ent, ent.PuzzGrid.GetDirectionVector(dir), force, turn: turn)
                .One().ToList<GridAction>(),
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
        _turn.OriginalOrientation = Ent.transform.rotation;
        
        var dir = GridEntity.GetDirection(MovementVec);
        Ent.PendingMoves.Add(this);

        if (_turn.Enabled)
        {
            Ent.transform.rotation = _turn.TargetRotation;
        }

        Ent.transform.position += MovementVec;
    }


    public override void Undo()
    {
        if (_aborted) return;
        Ent.PendingMoves.Remove(this);
        var dir = GridEntity.GetDirection(-MovementVec);

        Ent.transform.position -= MovementVec;

        if (_turn.Enabled) { Ent.transform.rotation = _turn.OriginalOrientation; }
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
        
        if (_turn.Enabled)
        {
            Ent.transform.rotation = _turn.OriginalOrientation;
            tasks.Add(Ent.transform.DORotate(_turn.TargetRotation.eulerAngles, 0.1f).SetEase(Ease.InOutSine).ToUniTask());
        }
        
        tasks.Add(Ent.transform.DOMove(Ent.transform.position + MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask());
        
        await UniTask.WhenAll(tasks);
    }

    public async override UniTask GetUndoTask()
    {
        Ent.transform.position += MovementVec;

        var tasks = new List<UniTask>();
        
        if (_turn.Enabled)
        {
            tasks.Add(Ent.transform.DORotate(_turn.OriginalOrientation.eulerAngles, 0.1f).SetEase(Ease.InOutSine).ToUniTask());
        }
        
        tasks.Add(Ent.transform.DOMove(Ent.transform.position - MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask());
        
        await UniTask.WhenAll(tasks);
    }
}
#endif