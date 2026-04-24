#if UNITASK && ODIN_INSPECTOR && UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using stoogebag.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class PuzzGrid : MonoBehaviour
{
    public event Action OnMoveFinished;
    public IObservable<Unit> OnMoveFinishedObservable() => Observable.FromEvent(x => OnMoveFinished += x, x => OnMoveFinished -= x);

    public event Action OnUndoFinished;
    public IObservable<Unit> OnUndoFinishedObservable() => Observable.FromEvent(x => OnUndoFinished += x, x => OnUndoFinished -= x);
    
    Guid guid = Guid.NewGuid();
    public string PuzzleName;

    public List<GridEntity> Entities { get; set; }

    private int CurrentTick = 0;
    public ActionQueue MoveQueue { get; set; }


    private void Awake()
    {
        // ResetGrid();
        MoveQueue = GetComponent<ActionQueue>();
        Entities = GetComponentsInChildren<GridEntity>().ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ReloadLevel();
        if (Input.GetKeyDown(KeyCode.Alpha9))
            LoadLevelAtIndex(0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(StageWon)
            NextLevel();
        }

        // if (Input.GetKeyDown(KeyCode.Minus))
        //     PrevLevel();
    }
    

    public async UniTask AddActionSetGroup(GridActionSetGroup actionSetGroup)
    {
        if (Paused) return;
        if (StageWon) return;
        if (_undoing) return;
        if (StageLost) return;

        _moving = true;

        var moveSummary = await RunActionSetGroup(actionSetGroup, GridActionSetGroup.GroupType.UserInput);


        while (true)
        {
            //break; //for debug
            
            CheckLossConditions();
            CheckWinConditions();
            //
            var settle = GetSettlementMoves(moveSummary);

            var settleSummary = await RunActionSetGroup(settle, GridActionSetGroup.GroupType.Settlement);
            //todo: do i need to loop this? what if there are chains of settlement? idk.

            var grav = GetGravityMoves();
            var gravSummary = await RunActionSetGroup(grav, GridActionSetGroup.GroupType.Gravity);

            if ((gravSummary == null || gravSummary.ExecutedMoveSummary.Count == 0) &&
                (settleSummary == null || settleSummary.ExecutedMoveSummary.Count == 0))
                break;

            //break;
        }

        _moving = false;
    }

    private async UniTask<GridActionSummary> RunActionSetGroup(GridActionSetGroup actionSetGroup,
        GridActionSetGroup.GroupType type)
    {
        EvaluateActionSetGroup(actionSetGroup, type);

        var approved = actionSetGroup.GetApprovedActions().ToList();
        var summary = actionSetGroup.GetActionSummary();
        if (!approved.Any())
        {
            await ExecuteAnimations(summary);
            return null;
        }

        while (true)
        {
            var sideEffects = GetSideEffectMoves(summary);

            foreach (var gridActionSet in approved)
            {
                gridActionSet.Execute();
            }

            var count = 0;
            if (sideEffects?.ActionSets?.Any() == true)
            {
                count++;
                if (count > 100) { } //for debug

                sideEffects = FilterSideEffects(sideEffects, approved);
                EvaluateActionSetGroup(sideEffects, GridActionSetGroup.GroupType.SideEffect);

                var approvedSideEffects = sideEffects.GetApprovedActions();
                if (approvedSideEffects == null || approvedSideEffects.Count() == 0) break;
                var sideEffectsSummary = sideEffects.GetActionSummary();

                approved.AddRange(approvedSideEffects);
                summary.Add(sideEffectsSummary);
                foreach (var sideEffect in approvedSideEffects)
                {
                    sideEffect.Execute();
                }
                
                foreach (var gridActionSet in approved)
                {
                    gridActionSet.Undo();
                }
            }
            else break;
        }

        foreach (var gridEntity in Entities)
        {
            gridEntity.PendingMoves.Clear();
        }

        await ExecuteAnimations(summary);

        _executedActions.Add(new GridActionResult(){Actions = approved, SummarisedMoves = summary, Type = type});

        //var approvedGp = new GridActionSetGroup(this) { ActionSets = approved, Type = type };
        //if (approvedGp.ActionSets.Count > 0) _actionSetGroups.Add(approvedGp);
        //return approvedGp;
        
        //wait a frame before firing the event since i think the rigidbody move doesn't update the transform pos
        //there's probably a better way. this could be moved to the portal check if it has to be.
        await UniTask.NextFrame();
        
        OnMoveFinished?.Invoke();
        return summary;
    }

    private void CheckWinConditions()
    {
        return; //todo. WinConditionProvider or something like that.
    }

    private void CheckLossConditions()
    {
        if (mangEnt == null) mangEnt = FindObjectOfType<MangEnt>();

    }

    public int LossY = 11;
    private WinEnt winEnt;
    private MangEnt mangEnt;

    bool _moving = false;
    private bool _undoing = false;

    private async UniTask Undo()
    {
        if (_moving || _undoing) return;
        _undoing = true;
        while (_executedActions.Count > 0)
        {
            var lastGp = _executedActions[^1];

            //var approved = lastGp.ActionSets.Where(t => t.Approved).ToList(); //todo:unnecessary i think, who cares.

            foreach (var set in lastGp.Actions)
            {
                set.Undo();
            }

            //todo:redo this
            //await UniTask.WhenAll(GridActionSet.GetUndoTasks(approved));

            await ExecuteUndoAnimations(lastGp.SummarisedMoves);
            
            //await UniTask.WhenAll(GridActionSet.GetUndoTasks(lastGp.Actions.SelectMany(t=>t.)));
            _executedActions.Remove(lastGp);
            CheckLossConditions();
            CheckWinConditions();

            if (lastGp.Type == GridActionSetGroup.GroupType.UserInput) break;
        }

        
        OnUndoFinished?.Invoke();
        _undoing = false;
    }

    public async UniTask UndoAll()
    {
        if (_moving || _undoing) return;

        TimeScaleManager.Instance.TweenTimeScale(10, 4);

        while (_executedActions.Count > 0)
        {
            await Undo();
        }

        TimeScaleManager.Instance.SetTimeScale(1);
    }


    //this evaluates the actionsetgroup including required consequences and NOT side effects 
    //returns the approved UNEXECUTED moves.
    private void EvaluateActionSetGroup(GridActionSetGroup actionSetGroup, GridActionSetGroup.GroupType type)
    {
        var approved = actionSetGroup.ActionSets.Where(t => t.Approved);

        foreach (var actionSet in actionSetGroup.ActionSets)
        {
            var actions = FilterSideEffects(actionSet, approved.ToList());
            actionSet.Actions = actions;
            actionSet.ResultActionSet = actionSet.Evaluate();
            if (actionSet.Approved)
            {
                actionSet.ResultActionSet.Execute();
            }
            approved = actionSetGroup.ActionSets.Where(t => t.Approved);
        }

        //roll back 
        foreach (var set in approved) 
        {
            set.ResultActionSet.Undo();
        }
    }

    //the purpose of this is to resolve cases where side effects are in conflict with consequences. eg something getting double-pushed.
    //it is unclear the final shape of this but we will just wing it for now.
    private GridActionSetGroup FilterSideEffects(GridActionSetGroup sideEffectMoves, List<GridActionSet> approved)
    {
        var sideEffectGroups = sideEffectMoves.ActionSets
            .SelectMany(t => t.Actions)
            .GroupBy(t => t.Ent)
            .ToDictionary(t => t.Key, t => t.ToList());

        var gp = new GridActionSetGroup(this);
        foreach (var (ent, effects) in sideEffectGroups)
        {
            var result = ent.FilterSideEffects(effects);
            gp.ActionSets.Add(new GridActionSet(this) { Actions = result.ToList() });
        }

        return gp;
    }
    private List<GridAction> FilterSideEffects(GridActionSet sideEffectMoves, List<GridActionSet> approved)
    {
        var sideEffectGroups = sideEffectMoves
            .Actions
            .Where(t => t.Ent != null) //HACK WARNING! spawn actions have no entity, but also no side effects. BEWARE!!!
            .GroupBy(t => t.Ent)
            .ToDictionary(t => t.Key, t => t.ToList());

        var actions = new List<GridAction> { };
        foreach (var (ent, effects) in sideEffectGroups)
        {
            var result = ent.FilterSideEffects(effects);
            actions.AddRange(result);
        }

        actions.AddRange(sideEffectMoves.Actions.Where(t=>t.Ent == null));
        
        return actions;
    }

    private async UniTask ExecuteAnimations(GridActionSummary summary)
    {
        if(summary == null) return;
        var tasks = new List<UniTask>();

        //executed moves
        foreach (var gridActions in summary.ExecutedMoveSummary.GroupBy(t => t.Ent))
        {
            foreach (var gridAction in gridActions)
            {
                bool overridden = false;
                foreach (var provider in gridAction.Ent.gameObject.GetComponentsWithInterface<IActionExecuteOverrideProvider>())
                {
                    (var exists, var newTask) = provider.GetExecutionTaskOverride(gridAction);
                    if (exists)
                    {
                        tasks.Add(newTask);
                        overridden = true;
                    }
                }
                
                if(!overridden){ tasks.Add( gridAction.GetExecutionTask());}
            }
        }

        foreach (var gridActions in summary.FailedMoveSummary.GroupBy(t => t.Ent))
        {
            foreach (var gridAction in gridActions)
            {
                var task = gridAction.GetFailureTask();
                tasks.Add(task);
            }
        }
        
        await UniTask.WhenAll(tasks);
    }

    private async UniTask ExecuteUndoAnimations(GridActionSummary summary)
    {
        var tasks = new List<UniTask>();
        if(summary == null) return;

        foreach (var gridActions in summary.ExecutedMoveSummary.GroupBy(t => t.Ent))
        {
            foreach (var gridAction in gridActions)
            {
                bool overridden = false;
                foreach (var provider in gridAction.Ent.gameObject
                             .GetComponentsWithInterface<IActionExecuteOverrideProvider>())
                {
                    (var exists, var newTask) = provider.GetUndoTaskOverride(gridAction);
                    if (exists)
                    {
                        tasks.Add(newTask);
                        overridden = true;
                    }
                }

                if (!overridden)
                {
                    tasks.Add(gridAction.GetUndoTask());
                }
            }
        }
        await UniTask.WhenAll(tasks);
    }


    private List<GridActionResult> _executedActions = new List<GridActionResult>();


    public GridActionSetGroup GetSideEffectMoves(GridActionSummary summary)
    {
        var approvedActions = summary.ExecutedMoveSummary;
        GridActionSetGroup gp = null;
        foreach (var ent in Entities)
        {
            var cons = ent.GetSideEffectMoves(approvedActions);
            if (cons != null)
            {
                if (gp == null) gp = new GridActionSetGroup(this);
                gp.ActionSets.Add(cons);
            }
        }

        return gp;
    }

    public GridActionSetGroup GetSettlementMoves(GridActionSummary actionSummary)
    {
        var gp = new GridActionSetGroup(this);

        foreach (var ent in Entities)
        {
            var cons = ent.GetSettlementMoves(actionSummary);
            if (cons != null) gp.ActionSets.AddRange(cons);
            
            foreach (var settlementMoveProvider in ent.gameObject.GetComponentsWithInterface<ISettlementMoveProvider>())
            {
                   var spCons = settlementMoveProvider.GetSettlementMoves(actionSummary);
                   if (spCons != null) gp.ActionSets.Add(spCons);
            }
        }

        return gp;
    }

    public GridActionSetGroup GetGravityMoves()
    {
        var gp = new GridActionSetGroup(this);

        foreach (var entity in Entities.Where(t=>t.gameObject.activeSelf))
        {
            var grav = entity.GetGravityMoves();
            if (grav == null) continue;
            gp.ActionSets.AddRange(grav.ActionSets);
        }

        return gp;
    }

    public static float GridSpacing()
    {
        return 10f ;
    }

    public Vector3 GetDirectionVector(Vector3 direction)
    {
        return transform.TransformVector(direction);
    }
}

public interface ISettlementMoveProvider
{
    GridActionSet GetSettlementMoves(GridActionSummary actionSummary);
}

public interface IConsequenceProvider
{
    GridActionSetGroup GetConsequences(GridActionSet triggeringSet, IEnumerable<GridAction> triggeringActions);
}

#endif