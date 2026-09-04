using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ChangeTracker
{
    static ChangeTracker()
    {
        // This monitors every property change in the entire editor
        Undo.postprocessModifications += OnModification;
    }

    static UndoPropertyModification[] OnModification(UndoPropertyModification[] modifications)
    {
        foreach (var mod in modifications)
        {
            // Check if the property being changed is the Active state
            if (mod.currentValue.propertyPath == "m_IsActive")
            {
                //Debug.Log($"<color=yellow>Property 'Active' changed on: {mod.currentValue.target.name}</color>");
                // This won't give a code stack trace, but it confirms IF the editor is doing it
            }
        }
        return modifications;
    }
}