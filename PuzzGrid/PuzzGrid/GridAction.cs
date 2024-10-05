using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;

using UnityEngine.Rendering;

public abstract class GridAction
{
    public abstract void
        Execute(); //this can be a single GAS with all the natural 'groups' in it, since if one fails all fail.

    //public abstract UniTask ExecuteFinally();
    public abstract void Undo();
    
    //returns the 'approved part' of the action. if null, rejected.
    public abstract GridAction Evaluate(HashSet<GridAction> evaluatedActions);

    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();
    
    //moved to the entity!
    //public abstract IEnumerable<UniTask> GetExecutionTasks();
    //public abstract IEnumerable<UniTask> GetUndoTasks();

    public int ID;

    public GridEntity Ent;
    //public abstract GridAction ResolveConflict(GridAction sideEffectAction);
    public abstract bool ConflictsWith(GridAction sideEffectAction);

    public bool Executed;
    public bool Evaluated;
    public Approvals Approval = Approvals.Unevaluated;
    
    //this collates all the actions/consequences that are approved/failed. usually it will just be returning myself.
    //eg CompoundMoveAction must group all the consequent SimpleMoveActions etc.
    public virtual IEnumerable<GridAction> GetApprovalSummary(bool approved)
    {
        if(approved) yield return this;
    }
    public virtual IEnumerable<GridAction> GetFailureSummary(bool approved)
    {
        if(!approved) yield return this;
    }

    public List<GridAction> Consequences = new List<GridAction>();

    public virtual UniTask GetExecutionTask() => UniTask.CompletedTask;
    public virtual UniTask GetFailureTask() => UniTask.CompletedTask;
    public virtual UniTask GetUndoTask() => UniTask.CompletedTask;
    
    
    
    protected void Approve()
    {
        Approval = Approvals.Approved;
        Evaluated = true;
    }
    
}

public class GridActionSet
{
    public List<GridAction> Actions = new List<GridAction>();
    public PuzzGrid PuzzGrid;

    public GridActionSet(PuzzGrid grid)
    {
        PuzzGrid = grid;
    }

    public bool Rejected => Evaluated && !Approved;
    public GridActionSet ResultActionSet { get; set; }

    //returns whether the action is approved, and if so an
    //actionset containing any dependencies. 
    public GridActionSet Evaluate()
    {
        if (!Actions.Any()) return null;

        var dependencies = new HashSet<GridAction>();
        var resultActions = new List<GridAction>();
        
        foreach (var action in Actions)
        {
            var eval = action.Evaluate(dependencies);
            Evaluated = true;
            
            
            if (eval == null) return null;
            
            resultActions.Add(eval);
            
            
            
            dependencies.AddRange(eval.GetAllDescendants<GridAction>(t=> t.Consequences));
        }

        foreach (var action in Actions)
        {
            dependencies.Remove(action);
        }
        
        var dependenciesGAS = dependencies.Any() ? new GridActionSet(PuzzGrid) { Actions = dependencies.ToList() } : null;

        if (dependenciesGAS != null)
        {
            dependenciesGAS.Approved = true; //BC. this might need more interesting logic.
        }
        
        Evaluated = true;
        Approved = true;
        Consequences = dependenciesGAS;
        return new GridActionSet(PuzzGrid) { Actions = resultActions, Consequences = dependenciesGAS,};
    }

    public void Execute()
    {
        foreach (var action in Actions)
        {
            action.Execute();
        }

        if (Consequences != null)
            Consequences.Execute();
    }

    public void Undo()
    {
        foreach (var action in Actions)
        {
            action.Undo();
        }

        if (Consequences != null)
            Consequences.Undo();

        //BC: this may be in the wrong order. i think it doesn't matter.
    }


    public bool Approved = false;
    public bool Evaluated = false;
    public bool Executed = false;
    public GridActionSet Consequences;


    public static GridActionSet GetSingle(PuzzGrid grid, GridAction cma)
    {
        return new GridActionSet(grid) { Actions = { cma } };
    }

    public static void Include(PuzzGrid grid, GridAction action, ref GridActionSet set)
    {
        if (set == null)
        {
            set = new GridActionSet(grid) { Actions = { action } };
        }
        else set.Actions.Add(action);

    }

    public GridActionSummary GetActionSummary()
    {
        GridActionSummary summary = null;
        foreach (var gridAction in ResultActionSet?.Actions ?? Actions) //todo. decide if we should be using this, or CALLING resultactionset...
        {
            summary = summary ?? new GridActionSummary();
            var app = gridAction.GetApprovalSummary(this.Approved);
            var fail = gridAction.GetFailureSummary(this.Approved);
            
            summary.ExecutedMoveSummary.AddRange(app);
            summary.FailedMoveSummary.AddRange(fail);
        }

        if (Consequences!= null)
        {
            foreach (var cons in Consequences.ResultActionSet?.Actions ?? Consequences.Actions)
            {
                
                var app = cons.GetApprovalSummary(Consequences.Approved);
                var fail = cons.GetFailureSummary(Consequences.Approved);
                
                summary.ExecutedMoveSummary.AddRange(app);
                summary.FailedMoveSummary.AddRange(fail);
            }
        }

        
        return summary;
    }
}

// public class GridActionSetGroupChain
// {
//     public PuzzGrid PuzzGrid;
//     public List<Func<GridActionSetGroup>> ActionGetters;
//     public Func<GridActionSetGroup, GridActionSetGroup> ReverseActionGetters;
//
//     public GridActionSetGroupChain(PuzzGrid puzzGrid, List<Func<GridActionSetGroup>> actions,
//         Func<GridActionSetGroup, GridActionSetGroup> getReverseActionFunc)
//     {
//         PuzzGrid = puzzGrid;
//         ActionGetters = actions;
//         ReverseActionGetters = getReverseActionFunc;
//     }
// }

public class GridActionResult
{
    public List<GridActionSet> Actions;
    public GridActionSetGroup.GroupType Type;
    public GridActionSummary SummarisedMoves { get; set; }
}

public class GridActionSummary
{
    public List<GridAction> ExecutedMoveSummary { get;  } = new();
    public List<GridAction> FailedMoveSummary { get;  } = new();

    public void Add(GridActionSummary other)
    {
        if(other == null) return;
        ExecutedMoveSummary.AddRange(other.ExecutedMoveSummary);
        FailedMoveSummary.AddRange(other.FailedMoveSummary);
    }
}