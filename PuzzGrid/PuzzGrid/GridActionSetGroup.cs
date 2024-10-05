using System.Collections.Generic;
using System.Linq;

public class GridActionSetGroup
{
    public GridActionSetGroup(PuzzGrid puzzGrid)
    {
        PuzzGrid = puzzGrid;
    }

    public List<GridActionSet> ActionSets = new List<GridActionSet>();

    public static GridActionSetGroup Create(PuzzGrid grid, IEnumerable<GridActionSet> sets)
    {
        if (sets is List<GridActionSet> l)
            return new GridActionSetGroup(grid) { ActionSets = l };
        return new GridActionSetGroup(grid) { ActionSets = sets.ToList() };
    }


    public PuzzGrid PuzzGrid;

    public GridActionSet Merge()
    {
        var merged = new GridActionSet(PuzzGrid);
        foreach (var actionSet in ActionSets)
        {
            merged.Actions.AddRange(actionSet.Actions);
        }

        return merged;
    }

    public GroupType Type;

    public enum GroupType
    {
        UserInput,
        SideEffect,
        Settlement,
        Gravity,
    }



    public static GridActionSetGroup GetSingle(GridActionSet actionSet)
    {
        return new GridActionSetGroup(actionSet.PuzzGrid) { ActionSets = new List<GridActionSet> { actionSet } };
    }

    public static GridActionSetGroup GetSingle(PuzzGrid grid, GridAction action)
    {
        return new GridActionSetGroup(grid)
        {
            ActionSets = new List<GridActionSet>
            {
                GridActionSet.GetSingle(grid, action)
            }
        };
    }


    public GridActionSummary GetActionSummary()
    {
        GridActionSummary summary = null;
        foreach (var set in ActionSets)
        {
            if (summary == null) summary = new GridActionSummary();
            
            summary.Add(set.GetActionSummary());
            
        }

        return summary;
    }

    public IEnumerable<GridActionSet> GetApprovedActions()
    {
        return ActionSets.Where(t => t.Approved).Select(t=>t.ResultActionSet);
    }
}