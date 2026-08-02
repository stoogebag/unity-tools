#if UNITASK && ODIN_INSPECTOR && UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;


public struct TurnData
{
    public Quaternion TargetRotation;
    public Quaternion OriginalOrientation;
    public bool Enabled => TargetRotation != default;
    public static readonly TurnData None = default;
    public static TurnData FaceDirection(Vector3 direction) => new TurnData
    {
        TargetRotation = Quaternion.LookRotation(direction, Vector3.up)
    };
}

public class SimpleMoveAction : GridAction, IPushAction
{
    public PushForce Force { get; }
    public Vector3 MovementVec { get; }

    static int _idCounter = 0;
    private bool _aborted;

    private TurnData _turn;
    public TurnData Turn { get => _turn; set => _turn = value; }

    public SimpleMoveAction(GridEntity ent, Vector3 movementVec, PushForce force, bool isPlatformPush = false,
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
            Actions = new SimpleMoveAction(ent, ent.PuzzGrid.GetDirectionVector(dir), force, turn: turn)
                .One().ToList<GridAction>(),
        };
    }

    //evaluated actions prevents us from moving something twice in one actionset...
    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;

        var evalProviders = Ent.gameObject.GetComponentsWithInterface<IMoveEvaluationProvider>();
        foreach (var eval in evalProviders)
        {
            if (eval.GetActionEvaluationOverride(this, ref evaluatedActions).Result ==
                ActionEvaluationOverrideResult.ActionOverrideResultTypes.Reject)
            {
                Approval = Approvals.Failed;
                return null;
            }
        }


        if (!Ent.isActiveAndEnabled)
        {
            Approval = Approvals.Failed;
            return null;
        }

    if (Ent is MangEnt || Ent is BlockEnt || Ent is MangEntOriented || Ent is BarrelEnt || Ent.CanMove)
        {
            var hits = new List<PuzzGridRaycastResult>();

            
            foreach ((var rayOrigin, var buffer) in Ent.GetFrontierRayOrigins(MovementVec, PuzzGrid.GridSpacing())
                         .ToList())
            {
                var rayDir = MovementVec + MovementVec.normalized * buffer;
                if (Ent.PuzzGrid.DrawDebug) Debug.DrawRay(rayOrigin, rayDir, Color.red, 1f);

                //cast.
                if (Physics.Raycast(rayOrigin, rayDir, out var hit, PuzzGrid.GridSpacing() * 100,
                        LayerMask.GetMask("Default"))) //todo: make layermask configurable
                {
                    var hitDistance = hit.distance - buffer;
                    if (hitDistance >= MovementVec.magnitude) continue;

                    //portal related stuff.
                    Vector3 portalOutDirection = Vector3.zero;
                    float prePortalDistance = 0;
                    float postPortalDistance = 0;
                    float portalMultiplier = 1;

                    var hitEnt = hit.collider.gameObject.GetComponentInAncestor<GridEntity>();

                    if (hitEnt == null) continue;
                    if (hitEnt == Ent) continue;

                    var res = new PuzzGridRaycastResult()
                    {
                        HitEnt = hitEnt,
                        PrePortalDistance = prePortalDistance,
                        PostPortalDistance = postPortalDistance,
                        PortalMultiplier = portalMultiplier,
                        PortalOutDirection = portalOutDirection,
                        HitDistance = hitDistance,
                        ThroughPortal = postPortalDistance > 0,
                    };
                    hits.Add(res);
                }
            }

            if (!hits.Any())
            {
                //no hits! fully approved.
                this.Evaluated = true;
                this.Approval = Approvals.Approved;
                return this;
            }

            
            //LADDER
            if (this.Ent is MangEnt)
            {
                var ladderHit = hits.FirstOrDefault(t => t.HitEnt is LadderEnt);
                if (ladderHit != null)
                {
                    var ladder = ladderHit.HitEnt as LadderEnt;
                    if (ladderHit.HitDistance < 0.05f)
                    {
                        Debug.Log("ladder.");
                        if (ladder.transform.right.normalized.EqualsApprox(MovementVec.normalized))
                        {
                            Debug.Log("right direction.");
                            return new SimpleMoveAction(Ent, Vector3.up * 10, PushForce.Climb, false, TurnData.FaceDirection(MovementVec));
                        }
                    }
                }
            }

            var candidates = new List<SimpleMoveAction>();
            foreach (var gp in hits.GroupBy(t => t.HitEnt))
            {
                //BC: at the moment we seem to be just handling the first one. it's not right but it is ok for now.
                //todo: figure out if and when this is actually an issue and handle it.
                var res = gp.MinItem(t => t.HitDistance);
                SimpleMoveAction pushMove;
                SimpleMoveAction succeededMove = null;

                if (res.ThroughPortal)
                {
                    //we're not doing this atm.
                }
                else
                {
                    var hitDistance = Mathf.Max(0, res.HitDistance);
                    var hitEnt = res.HitEnt;

                    var movedVec = MovementVec.normalized * (hitDistance);
                    var pushVec = MovementVec.normalized * (this.MovementVec.magnitude - hitDistance);

                    if (hitDistance > 0.001f)
                        succeededMove = new SimpleMoveAction(hitEnt, movedVec, this.Force, this.PlatformPush);

                    if (pushVec.EqualsApprox(Vector3.zero))
                    {
                        var candidate = Merge(succeededMove, null);
                        candidates.Add(candidate);

                        continue;
                    }

                    pushMove = new SimpleMoveAction(hitEnt, pushVec, this.Force, this.PlatformPush);

                    //gets but does not evaluate consequences.
                    //NULL CONSEQUENCES MEANS APPROVAL.
                    var cons = hitEnt.GetConsequences(pushMove);

                    //it hasn't been evaluated, but might already have failed by dint of being
                    //a fail of a move (eg pushing a fixed wall)
                    if (cons?.Approval == Approvals.Failed)
                    {
                        var candidate = this.Merge(succeededMove, null);
                        candidates.Add(candidate);
                        continue;
                    }

                    if (cons?.Dependency == null)
                    {
                        candidates.Add(this);
                        continue;
                    }

                    if (evaluatedActions.Contains(cons.Dependency)) continue; //action is a duplicate
                    var passed = cons.Dependency.Evaluate(evaluatedActions);
                    if (passed == null)
                    {
                        var candidate = Merge(succeededMove, null);
                        candidates.Add(candidate);
                        continue;
                    }
                    else
                    {
                        var candidate = Merge(succeededMove, passed as SimpleMoveAction);
                        candidate.Consequences = new List<GridAction>() { passed };
                        candidates.Add(candidate);
                        continue;
                    }
                }
            }

            if (candidates.Count == 1)
            {
                candidates[0]?.Approve(); //bc: some gravity move is giving a null candidate. not sure why.
                return candidates[0];
            }

            if (candidates.Count == 0)
            {
                throw new Exception("no candidates?");
            }

            var shortestDistanceAllowed = candidates.Min(t => t?.MovementVec.magnitude ?? 0); //null is a zero

            if (shortestDistanceAllowed.EqualsApproximately(0))
            {
                return null;
            }
            else if (candidates.All(t => t.MovementVec.magnitude.EqualsApproximately(shortestDistanceAllowed, 0.001f)))
            {
                var result = candidates[0];

                //consolidate the consequences. each entity should be pushed at most ONCE
                var allConsequences = candidates.SelectMany(c => c.Consequences)
                    .OfType<SimpleMoveAction>()
                    .GetAllDescendants<SimpleMoveAction>(t => t.Consequences.OfType<SimpleMoveAction>())
                    .GroupBy(x => x.Ent)
                    .ToList();
                var finalConsequences = new List<GridAction>();
                foreach (var group in allConsequences)
                {
                    //we need the BIGGEST move. we have already tested and all the moves are possible and allowed, 
                    //so the biggest one is what should happen.
                    //we also need to flatten the consequences, ie remove them from the original moves and put them all here
                    //hopefully there's no nastiness that occurs...
                    var theMove = group.MaxItem(t => (t as SimpleMoveAction)?.MovementVec.magnitude ?? 0);
                    finalConsequences.Add(theMove);
                }

                foreach (var con in finalConsequences)
                {
                    con.Consequences = null;
                }

                result.Consequences = finalConsequences;

                result.Approve();
                return result;
            }
            else
            {
                var smallest = candidates.MinItem(t => t?.MovementVec.magnitude ?? 0);
                return smallest.Evaluate(evaluatedActions);
            }
        }


        Debug.Log("should you be here?");
        return null;
    }


    public SimpleMoveAction Merge(IEnumerable<SimpleMoveAction> actions)
    {
        var vec = Vector3.zero;
        foreach (var action in actions)
        {
            if (action == null) continue;
            vec += action.MovementVec;
        }

        if (vec == Vector3.zero) return null;
        var newMove = new SimpleMoveAction(Ent, vec, Force, PlatformPush, _turn);
        return newMove;
    }


    public SimpleMoveAction Merge(SimpleMoveAction a1, SimpleMoveAction a2)
    {
        return Merge(a1.ToEnumerable(a2));
    }

    public override void Execute()
    {
        // if (Ent.NodeEnts.First().CurrentNode != StartNode)
        // {
        //     //abort!
        //     _aborted = true;
        //     return;
        // }

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
        return "SimpleMoveAction: " + Ent.name + " move " + MovementVec;
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
        
        tasks.Add(Ent.transform.DOMove(Ent.transform.position + MovementVec, 0.2f).SetEase(Ease.InOutQuad).ToUniTask());
        
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

public interface IMoveEvaluationProvider
{
    ActionEvaluationOverrideResult GetActionEvaluationOverride(GridAction action, ref HashSet<GridAction> evaluatedActions);
}

public class ActionEvaluationOverrideResult
{
    public ActionOverrideResultTypes Result;

    public static ActionEvaluationOverrideResult Pass => new () { Result = ActionOverrideResultTypes.ApproveProvisional };

    public static ActionEvaluationOverrideResult Fail => new () { Result = ActionOverrideResultTypes.Reject }; 
    
    public enum ActionOverrideResultTypes
    {
        Reject,
        ApproveProvisional,
    }
}

public interface IActionExecuteOverrideProvider
{
    (bool, Action<GridAction>) GetExecuteOverride(GridAction gridAction);
    (bool, Action<GridAction>) GetUndoOverride(GridAction gridAction);
    (bool, UniTask) GetExecutionTaskOverride(GridAction gridAction);
    (bool, UniTask) GetUndoTaskOverride(GridAction gridAction);
}

public interface IPushAction
{
    Vector3 MovementVec { get; }

    PushForce Force { get; }
}

public class PuzzGridRaycastResult
{
    public bool ThroughPortal;
    public GridEntity HitEnt;


    public float PrePortalDistance;
    public float PostPortalDistance;
    public float PortalMultiplier;
    public Vector3 PortalOutDirection;
    public float HitDistance;
}

#endif