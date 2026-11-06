using UnityEngine;

/// <summary>
/// Base component for a one-click Houdini task.
///
/// It holds only the connection to a control node in the open hip. The concrete
/// subclass holds the task's own inputs, and a matching editor subclass (see
/// <c>stoogebag.houdinipipeline.tasks.editor</c>) does the work.
///
/// See <c>Assets/Boomer/Docs/HoudiniTasks.md</c> for the hip contract.
/// </summary>
public abstract class HoudiniTask : MonoBehaviour
{
    [Tooltip("Path to the control node in the open hip, e.g. /obj/geo1/unity_task.")]
    public string nodePath = "/obj/geo1/unity_task";

    [Tooltip("Name of the ROP FBX sibling that exports the result.")]
    public string ropName = "rop_fbx";

    [Tooltip("Optional output folder override. Tokens: {sourceDir} {name} {task}. Empty uses the project settings pattern.")]
    public string outputFolderOverride = "";

    /// <summary>Short name used in paths and logs. No spaces or slashes.</summary>
    public abstract string TaskName { get; }
}
