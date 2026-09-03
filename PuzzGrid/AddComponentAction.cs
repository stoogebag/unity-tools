using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITASK
#if ODIN_INSPECTOR
#if UNIRX
public class AddComponentAction<T> : GridAction where T: MonoBehaviour 
{
    public override void Execute()
    {
        var t = Ent.gameObject.AddComponent<T>();
        (t as IGridEntityComponent).Entity = Ent;
    }

    public override void Undo()
    {
        Object.Destroy(Ent.gameObject.GetComponent<T>());
    }

    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;
        return this;
    }

    public override bool ConflictsWith(GridAction sideEffectAction) => false;
}

public class SpawnEntityAction : GridAction
{
    internal GridEntity EntityPrefab;

    internal PuzzGrid Grid;
    internal Vector3 Position;
    internal Quaternion Rotation;
    
    public override void Execute()
    {
        var obj = Object.Instantiate(EntityPrefab, Position, Rotation, Grid.transform);
        Ent = obj.GetComponent<GridEntity>();
        Grid.Entities.Add(Ent);
    }

    public override void Undo()
    {
        Grid.Entities.Remove(Ent);
        Object.Destroy(Ent.gameObject);
    }

    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;
        return this;
    }

    public override bool ConflictsWith(GridAction sideEffectAction) => false;
}


public class DisableEntityAction : GridAction
{
    
    public override void Execute()
    {
        
        Ent.gameObject.SetActive(false);
    }

    public override void Undo()
    {
        Ent.gameObject.SetActive(true);
    }

    public override GridAction Evaluate(HashSet<GridAction> evaluatedActions)
    {
        Evaluated = true;
        return this;
    }

    public override bool ConflictsWith(GridAction sideEffectAction) => false;
}


#endif
#endif
#endif