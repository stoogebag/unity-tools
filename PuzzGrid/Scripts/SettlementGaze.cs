using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GridEntity))]
public class SettlementGaze : MonoBehaviour,  IGridEntityComponent
{

    private void Awake()
    {
        Entity = GetComponent<GridEntity>();
    }

    [SerializeField] internal SettlementGaze _partner = null;

    public GridActionSet GetSettlementMoves(GridActionSummary actionSummary)
    {
        //get forwards neighbours. 
        var raycast = Entity.GetNeighbours(Entity.transform.forward * 1000);
        
        var first = raycast?.FirstOrDefault(t => t.HitEnt?.GetComponent<SettlementGaze>() != null);
        var potentialPartner = first?.HitEnt.GetComponent<SettlementGaze>();

        if (first?.HitEnt != null)
        {
            var returnRaycast = first.HitEnt.GetNeighbours(first.HitEnt.transform.forward * 1000);
            var me = returnRaycast?.FirstOrDefault(t => t?.HitEnt == Entity);
            if(me == null) potentialPartner = null; //we're not looking at each other
        }
        
        if (potentialPartner == _partner) return null;
        
        var actionSet = new GridActionSet(Entity.PuzzGrid);
        
        //if we have a partner already, dissolve that first
        if (_partner != null)
        {
            actionSet.Actions.Add(new PartnershipDissolveGridAction(this, _partner));
            
        }

        if (potentialPartner != null)
        {
            actionSet.Actions.Add(new PartnershipEstablishGridAction(this, potentialPartner));
        }
        return actionSet;
        return null;
        
    }

    public GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set)
    {
        if (_partner == null) return null;        
        GridActionSet result = null;

        foreach (var gridAction in set)
        {
            if (gridAction is SimpleMoveAction move)
            {
                if (move.Ent.TryGetComponent<SettlementGaze>(out var gaze))
                {
                    if (gaze.Entity == Entity) continue;
                    
                    
                    if (move.Ent == _partner?.Entity)
                    {
                        var se = SimpleMoveAction.GetMove(Entity, move.MovementVec, move.Force, false, _partner.transform.rotation);
                        //var cma = new CompoundMoveAction(Entity, move.MovementVec, count, true, false, move.Force);
                        GridActionSet.Include(Entity.PuzzGrid, se.Actions[0], ref result); 
                    }
                }
            }
        }

        return result;
        
        // var gaze = GetComponent<SettlementGaze>();
        // if (gaze != null && gaze._partner != null)
        // {
        //     
        //     //this is bad. i need to refactor. i think settlementprovider is redundant and IGridEntityComponent is probably the right way to go. OR, not lol.
        //     foreach (var gridAction in set)
        //     {
        //         if (gridAction.Ent != this) continue;
        //         if (gridAction is SimpleMoveAction moveAction)
        //         {
        //             var se = SimpleMoveAction.GetMove(gaze._partner.Entity, moveAction.MovementVec, moveAction.Force, false, gaze._partner.transform.rotation);
        //
        //             if (se != null)
        //             {
        //                 if (result == null) result = se;
        //                 else result.Actions.AddRange(se.Actions);
        //             }
        //         }
        //     }
        // }
        
    }

    public GridEntity Entity { get; set; }
}


public class PartnershipEstablishGridAction : GridAction
{

    private GridEntity PartnerEnt;
    
    public PartnershipEstablishGridAction(SettlementGaze initiator, SettlementGaze partner)
    {
        Ent = initiator.GetComponent<GridEntity>();
        PartnerEnt = partner.GetComponent<GridEntity>();
    }

    public override void Execute()
    {
        var initiatorGaze = Ent.GetComponent<SettlementGaze>();
        var partnerGaze = PartnerEnt.GetComponent<SettlementGaze>();
        initiatorGaze._partner = partnerGaze;
        partnerGaze._partner = initiatorGaze; //dodgy af
    }

    public override void Undo()
    {
        var initiatorGaze = Ent.GetComponent<SettlementGaze>();
        var partnerGaze = PartnerEnt.GetComponent<SettlementGaze>();
        initiatorGaze._partner = null;
        partnerGaze._partner = null; //dodgy af
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
        return Equals((PartnershipEstablishGridAction)obj);
    }

    public override int GetHashCode()
    {
        return Ent.GetHashCode() ^ PartnerEnt.GetHashCode();
    }

    public override bool ConflictsWith(GridAction sideEffectAction)
    {
        return false;
    }

    public override UniTask GetExecutionTask()
    {
        Debug.Log("partnership established between "+Ent.name+" and "+PartnerEnt.name);
        
        
        return UniTask.CompletedTask;
        //return UniTask.WhenAll(Enumerable.Select(Activateable.GameObject.GetComponents<IActivateTaskProvider>(), t=>t.GetActivateTask(Activateable.Activated.Value, false)));
    }
    
    public override UniTask GetUndoTask()
    {
        
        return UniTask.CompletedTask;
        //return UniTask.WhenAll(Enumerable.Select(Activateable.GameObject.GetComponents<IActivateTaskProvider>(), t=>t.GetActivateTask(Activateable.Activated.Value, true)));
    }
}



public class PartnershipDissolveGridAction : GridAction
{

    private GridEntity PartnerEnt;
    
    public PartnershipDissolveGridAction(SettlementGaze initiator, SettlementGaze partner)
    {
        Ent = initiator.GetComponent<GridEntity>();
        PartnerEnt = partner.GetComponent<GridEntity>();
    }

    public override void Execute()
    {
        var initiatorGaze = Ent.GetComponent<SettlementGaze>();
        var partnerGaze = PartnerEnt.GetComponent<SettlementGaze>();
        initiatorGaze._partner = null;
        partnerGaze._partner = null; //dodgy af
    }

    public override void Undo()
    {
        
        var initiatorGaze = Ent.GetComponent<SettlementGaze>();
        var partnerGaze = PartnerEnt.GetComponent<SettlementGaze>();
        initiatorGaze._partner = partnerGaze;
        partnerGaze._partner = initiatorGaze; //dodgy af
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
        return Equals((PartnershipDissolveGridAction)obj);
    }

    public override int GetHashCode()
    {
        return Ent.GetHashCode() ^ PartnerEnt.GetHashCode();
    }

    public override bool ConflictsWith(GridAction sideEffectAction)
    {
        return false;
    }

    public override UniTask GetExecutionTask()
    {
        Debug.Log("partnership Dissolved between "+Ent.name+" and "+PartnerEnt.name);
        
        
        
        return UniTask.CompletedTask;
        //return UniTask.WhenAll(Enumerable.Select(Activateable.GameObject.GetComponents<IActivateTaskProvider>(), t=>t.GetActivateTask(Activateable.Activated.Value, false)));
    }
    
    public override UniTask GetUndoTask()
    {
        
        return UniTask.CompletedTask;
        //return UniTask.WhenAll(Enumerable.Select(Activateable.GameObject.GetComponents<IActivateTaskProvider>(), t=>t.GetActivateTask(Activateable.Activated.Value, true)));
    }
}

