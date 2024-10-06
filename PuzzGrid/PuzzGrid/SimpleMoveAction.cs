using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;


public class SimpleMoveAction : GridAction, IPushAction
{
    public PushForce Force { get; }
    public Vector3 MovementVec { get; }


    static int _idCounter = 0;
    private bool _aborted;

    public SimpleMoveAction(GridEntity ent, Vector3 movementVec, PushForce force, bool isPlatformPush = false)
    {
        PlatformPush = isPlatformPush;
        Force = force;
        Ent = ent;
        MovementVec = movementVec;
        ID = _idCounter++; //todo: consider if this is a good idea. will they always be ordered by creation time?
    }

    public bool PlatformPush { get; set; }


    public static GridActionSet GetMove(GridEntity ent, Vector3 dir, PushForce force)
    {
        return new GridActionSet(ent.PuzzGrid)
        {
            Actions = new SimpleMoveAction(ent, ent.PuzzGrid.GetDirectionVector(dir), force).One().ToList<GridAction>(),
        };
    }

    List<SimpleMoveAction> _approvedMovements;

    //evaluated actions prevents us from moving something twice in one actionset...
    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        _approvedMovements = new List<SimpleMoveAction>();
        Evaluated = true;

        if (Ent is MangEnt || Ent is BlockEnt)
        {
            
            var hits = new List<PuzzGridRaycastResult>();
            
                foreach ((var rayOrigin, var buffer) in Ent.GetFrontierRayOrigins(MovementVec, PuzzGrid.GridSpacing())
                             .ToList())
                {
                    var rayDir = MovementVec + MovementVec.normalized * buffer;

                    Debug.DrawRay(rayOrigin, rayDir, Color.red, 1f);


                    //cast.
                    if (Physics.Raycast(rayOrigin, rayDir, out var hit, PuzzGrid.GridSpacing() * 100,
                            LayerMask.GetMask("Default")))
                    {
                        var hitDistance = hit.distance - buffer;
                        if (hitDistance >= MovementVec.magnitude) continue;

                        //portal related stuff.
                        Vector3 portalOutDirection = Vector3.zero;
                        float prePortalDistance = 0;
                        float postPortalDistance = 0;
                        float portalMultiplier = 1;


                        var hitEnt = hit.collider.gameObject.GetComponentInAncestor<GridEntity>();


                        //BC: instead of casting throught the portal, we will allow the push to apply to the clone. this might be unsustainable depending on funky portal locations. beware.

                        // if (hit.collider.gameObject.TryGetComponent<Portal>(out var portal))
                        // {
                        //     //the ray has passed through a portal, how spooky!
                        //     //FOR NOW: assume a maximum of one portal per ray lol.
                        //
                        //     var inTransform = portal.transform;
                        //     var outTransform = portal.OtherPortal.transform;
                        //  
                        //     Vector3 relativePos = inTransform.InverseTransformPoint(hit.point);
                        //     relativePos = PortalClone.halfTurn * relativePos;
                        //
                        //     var portalBuffer = 0.01f;
                        //     var newRayOrigin = outTransform.TransformPoint(relativePos + new Vector3(0,0,portalBuffer));
                        //
                        //     // Update rotation of clone.
                        //     // Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * Quaternion.Euler(dir);
                        //     // relativeRot =PortalClone.halfTurn * relativeRot;
                        //     // var newRayDir = (outTransform.forward * relativeRot);
                        //
                        //     var localRotationInPortal = inTransform.InverseTransformDirection(MovementVec);
                        //     var newWorldDirection = outTransform.TransformDirection(localRotationInPortal.MultiplyPointwise(1,1,-1));
                        //
                        //     portalMultiplier = (outTransform.lossyScale.x / inTransform.lossyScale.x);
                        //     prePortalDistance = hitDistance;
                        //     portalOutDirection = newWorldDirection.normalized;
                        //     var newRayLength = (MovementVec.magnitude - hitDistance) * portalMultiplier + portalBuffer;
                        //     
                        //     var newRay = new Ray(newRayOrigin, newWorldDirection*newRayLength);
                        //    
                        //     Debug.DrawRay(newRay.origin, newRay.direction*newRayLength, Color.magenta, 1f);
                        //
                        //     //cast a new ray! and see how we go.
                        //     //todo: do we need to do the buffer again? possibly.
                        //     if (Physics.Raycast(newRayOrigin, newRay.direction*newRayLength, out hit, PuzzGrid.GridSpacing() * 100))
                        //     {
                        //         hitDistance = hit.distance - portalBuffer;
                        //
                        //         var totalDistanceInPrePortalUnits = prePortalDistance + hitDistance / portalMultiplier;
                        //         if (totalDistanceInPrePortalUnits >= MovementVec.magnitude) continue;
                        //         
                        //         hitEnt = hit.collider.gameObject.GetComponentInAncestor<GridEntity>();
                        //     }
                        //     else continue;
                        // }
                        //


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
            

            if (!hits.Any()) {//no hits! fully approved.
                this.Evaluated = true;
                this.Approval = Approvals.Approved;
                return this; 
            }

            var candidates = new List<SimpleMoveAction>();
            foreach (var gp in hits.GroupBy(t=>t.HitEnt))
            {
                //BC: at the moment we seem to be just handling the first one. it's not right but it is ok for now.todo, fix

                var res = gp.MinItem(t=>t.HitDistance);
                SimpleMoveAction pushMove;
                SimpleMoveAction succeededMove = null;

                if (res.ThroughPortal)
                {
                    //we're not doing this atm.
                }
                else
                {
                    var hitDistance = Mathf.Max(0,res.HitDistance);
                    var hitEnt = res.HitEnt;

                    var movedVec = MovementVec.normalized * (hitDistance);
                    var pushVec = MovementVec.normalized * (this.MovementVec.magnitude - hitDistance);


                    if (hitDistance > 0.001f)
                        succeededMove = new SimpleMoveAction(hitEnt, movedVec, this.Force, this.PlatformPush);
                    
                    if (pushVec.EqualsApprox(Vector3.zero))
                    {
                        var candidate = Merge(succeededMove, null);
                        candidates.Add( candidate);

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
                        candidates.Add( candidate);
                        continue;
                    }

                    if (cons?.Dependency == null)
                    {
                        //this seems wrong. check
                        candidates.Add(this);
                        continue;
                    }
                    if (evaluatedActions.Contains(cons.Dependency)) continue; //action is a duplicate
                    var passed = cons.Dependency.Evaluate(evaluatedActions);
                    if (passed == null)
                    {
                        var candidate = Merge(succeededMove, null);
                        candidates.Add( candidate);
                        continue;
                    }
                    else
                    {
                        var candidate = Merge(succeededMove, passed as SimpleMoveAction);
                        candidate.Consequences = new List<GridAction>() { passed };
                        candidates.Add( candidate);
                        continue;
                    } 
                }
            }

            //
            //
            // foreach (var nodeEntity in gridNode.Entities)
            // {
            //     //this is perhaps inelegant. but we run 2 checks, first for summary 
            //     //rejection and then evaluating consequences if required. 
            //     var (pass, cons) = nodeEntity.GetConsequences(this);
            //     if (!pass) return false;
            //
            //     if (cons?.Dependency == null) continue;
            //
            //     if (!evaluatedActions.Add(cons.Dependency)) continue; //action is a duplicate
            //     var passed = cons.Dependency.Evaluate(ref evaluatedActions);
            //     if (!passed) return false;
            // }

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
                //todo: return a 'failed move' instead of null?
                return null;
            }
            else if (candidates.All(t=>t.MovementVec.magnitude.EqualsApproximately(shortestDistanceAllowed, 0.001f)))
            {
                var result = candidates[0];
                
                //consolidate the consequences. each entity should be pushed at most ONCE
                var allConsequences = candidates.SelectMany(c=>c.Consequences)
                    .OfType<SimpleMoveAction>()
                    .GetAllDescendants<SimpleMoveAction>(t=>t.Consequences.OfType<SimpleMoveAction>())
                    .GroupBy(x=>x.Ent)
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
                //return Merge(candidates[0], candidates[1]);
            }
            else
            {
                //simply rerun in the case where our move is cut short...
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
        var newMove = new SimpleMoveAction(Ent, vec, Force, PlatformPush);
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

        Ent.transform.position += MovementVec;

        // foreach (var ent in Ent.NodeEnts)
        // {
        //     ent.CurrentNode.EntityExited(ent, this);
        //     ent.CurrentNode = ent.CurrentNode.GetNeighbour(dir);
        //     //ent.transform.position = ent.CurrentNode.Position;
        //     if (ent.CurrentNode == null)
        //     {
        //     }
        //
        //     ent.CurrentNode.EntityEntered(ent, this);
        // }
    }


    public override void Undo()
    {
        if (_aborted) return;
        Ent.PendingMoves.Remove(this);
        var dir = GridEntity.GetDirection(-MovementVec);

        Ent.transform.position -= MovementVec;

        // foreach (var ent in Ent.NodeEnts)
        // {
        //     ent.CurrentNode.EntityExited(ent, this);
        //     ent.CurrentNode = ent.CurrentNode.GetNeighbour(dir);
        //     //ent.transform.position = ent.CurrentNode.Position;
        //     ent.CurrentNode.EntityEntered(ent, this);
        // }
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

    public override bool ConflictsWith(GridAction sideEffectAction) => false;

    public async override UniTask GetExecutionTask()
    {
       Ent.transform.position -= MovementVec;
       await Ent.transform.DOMove(Ent.transform.position+MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask();
    }
    
    public async override UniTask GetUndoTask()
    {
        Ent.transform.position += MovementVec;
        await Ent.transform.DOMove(Ent.transform.position-MovementVec, 0.1f).SetEase(Ease.InOutSine).ToUniTask();
    }
    
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