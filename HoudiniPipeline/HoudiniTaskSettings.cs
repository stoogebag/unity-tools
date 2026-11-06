using UnityEngine;

/// <summary>
/// Project-wide settings for Houdini tasks. There must be exactly one of these
/// assets in the project. See <c>Assets/Boomer/Docs/HoudiniTasks.md</c>.
/// </summary>
[CreateAssetMenu(menuName = "Boomer/Houdini Task Settings", fileName = "HoudiniTaskSettings")]
public class HoudiniTaskSettings : ScriptableObject
{
    [Tooltip("Absolute path of the hip the bridge should have open. Leave empty to skip the check.")]
    public string expectedHip = "";

    [Tooltip("Default output folder. Tokens: {sourceDir} {name} {task}.")]
    public string outputPattern = "{sourceDir}/Generated/{name}_{task}";

    [Tooltip("Houdini bridge base URL.")]
    public string bridgeUrl = "http://127.0.0.1:9876";
}
