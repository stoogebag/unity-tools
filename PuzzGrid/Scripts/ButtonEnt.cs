using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnt : GridEntity, IActivateable
{
    public GameObject GameObject => gameObject; //is there a better way?
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public override GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set) => null;

    public override GridActionSet GetSettlementMoves(GridActionSummary actionSummary)
    {
        //get upstairs neighbours. 
        var up = GetNeighbours(Vector3.up * 10);

        var pusher = up?.FirstOrDefault(t => t.HitEnt.GetComponent<IPushesButton>() != null);

        var swap = false;
        if (pusher == null)
        {
            if(Activated.Value) swap = true;
            //Pressed.Value = false;
        }
        else
        {
            if(!Activated.Value) swap = true;
            //Pressed.Value = true;
        }

        if (swap)
        {
            return GridActionSet.GetSingle(PuzzGrid, new ActivationGridAction(this));
        }
        else return null;
    }

    public override GridActionSetGroup GetGravityMoves() => null;

    public override GridActionConsequences GetConsequences(GridAction action)
    {
        if (action is IPushAction move)
        {
            return GridActionConsequences.ActionFailed;
        }

        return GridActionConsequences.ActionApproved;
    }

    public BoolReactiveProperty Activated { get; }= new BoolReactiveProperty();

    public event Action<bool> OnActiveChanged;
    public IObservable<bool> OnActiveChangedObservable => Observable.FromEvent<bool>(action => OnActiveChanged += action, action => OnActiveChanged -= action);
}

public class ActivationGridAction : GridAction
{
    private IActivateable Activateable;
    
    public ActivationGridAction(IActivateable activateable)
    {
        Activateable = activateable;
        Ent = activateable as GridEntity;
    }

    public override void Execute()
    {
        Activateable.Activated.Value = !Activateable.Activated.Value;
    }

    public override void Undo()
    {
        Activateable.Activated.Value = !Activateable.Activated.Value;
    }

    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;
        
        return this;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ActivationGridAction)obj);
    }

    public override int GetHashCode()
    {
        return Ent.GetHashCode();
    }

    public override bool ConflictsWith(GridAction sideEffectAction)
    {
        return false;
    }

    public override UniTask GetExecutionTask()
    {
        return UniTask.WhenAll(Activateable.GameObject.GetComponents<IActivateTaskProvider>().Select(t=>t.GetActivateTask(Activateable.Activated.Value, false)));
    }
    
    public override UniTask GetUndoTask()
    {
        return UniTask.WhenAll(Activateable.GameObject.GetComponents<IActivateTaskProvider>().Select(t=>t.GetActivateTask(Activateable.Activated.Value, true)));
    }
}

public interface IActivateTaskProvider
{
    public UniTask GetActivateTask(bool activeState, bool isUndo);
    //public UniTask GetDeactivateTask(bool activeState);
}

public interface IPushesButton
{
}

public interface IActivateable
{
    public GameObject GameObject { get; }
    public BoolReactiveProperty Activated { get; }
    
    //event Action<bool> OnActiveChanged;
    //IObservable<bool> OnActiveChangedObservable { get; } //think about this. 
}