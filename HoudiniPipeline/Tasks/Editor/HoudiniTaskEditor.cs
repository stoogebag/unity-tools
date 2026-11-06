using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Shared inspector for every <see cref="HoudiniTask"/>.
///
/// It draws the task's fields, the Generate button, the bridge/hip/node/ROP checks,
/// and the error handling. A concrete task subclasses this and implements
/// <see cref="RunTaskAsync"/>; it never has to repeat any of the above.
/// </summary>
public abstract class HoudiniTaskEditor : Editor
{
    const int TimeoutMinutes = 15;

    /// <summary>Does the task's work. Called after <see cref="ValidateAsync"/> passes.</summary>
    protected abstract UniTask RunTaskAsync(CancellationToken ct);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        var task = (HoudiniTask)target;
        if (GUILayout.Button($"Generate {task.TaskName}", GUILayout.Height(28)))
        {
            Run(task).Forget();
        }
    }

    async UniTask Run(HoudiniTask task)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(TimeoutMinutes));
        try
        {
            var settings = HoudiniTaskSettingsLocator.Find();
            HoudiniBridge.Url = settings.bridgeUrl;

            await ValidateAsync(task, settings, cts.Token);
            await RunTaskAsync(cts.Token);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[HoudiniTask] {task.TaskName} finished for '{task.name}'.", task);
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"[HoudiniTask] {task.TaskName} timed out after {TimeoutMinutes} minutes.", task);
        }
        catch (Exception e)
        {
            Debug.LogError($"[HoudiniTask] {task.TaskName} failed for '{task.name}': {e.Message}", task);
        }
    }

    /// <summary>
    /// Checks the bridge, the open hip, the control node, its required parms and the ROP.
    /// Every failure names the thing to fix.
    /// </summary>
    protected static async UniTask ValidateAsync(HoudiniTask task, HoudiniTaskSettings settings, CancellationToken ct)
    {
        var status = await HoudiniBridge.StatusAsync(ct);

        if (!string.IsNullOrWhiteSpace(settings.expectedHip) && !SamePath(status.hip, settings.expectedHip))
        {
            throw new InvalidOperationException(
                $"Houdini has '{status.hip}' open but settings expect '{settings.expectedHip}'. " +
                "Open the right hip, or fix Expected Hip on the Houdini Task Settings asset.");
        }

        var script = $@"
import hou
node = hou.node({HoudiniPython.Quote(task.nodePath)})
if node is None:
    raise RuntimeError('unity_task not found: ' + {HoudiniPython.Quote(task.nodePath)})
for parm_name in ('input_path', 'output_path'):
    if node.parm(parm_name) is None:
        raise RuntimeError('missing parm on ' + {HoudiniPython.Quote(task.nodePath)} + ': ' + parm_name)
rop = node.parent().node({HoudiniPython.Quote(task.ropName)})
if rop is None:
    raise RuntimeError('rop not found under ' + node.parent().path() + ': ' + {HoudiniPython.Quote(task.ropName)})
print('ok')
";
        await HoudiniBridge.ExecAsync(script, ct);
    }

    /// <summary>Compares two filesystem paths regardless of separators and case.</summary>
    protected static bool SamePath(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            return false;
        }

        return string.Equals(
            Path.GetFullPath(a).Replace('\\', '/').TrimEnd('/'),
            Path.GetFullPath(b).Replace('\\', '/').TrimEnd('/'),
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Turns a Unity asset path (<c>Assets/...</c>) into an absolute path Houdini can open.</summary>
    protected static string Absolute(string assetPath)
    {
        var projectRoot = Path.GetDirectoryName(Application.dataPath)?.Replace('\\', '/');
        return projectRoot + "/" + assetPath.Replace('\\', '/').TrimStart('/');
    }

    /// <summary>
    /// Expands <c>{sourceDir}</c>, <c>{name}</c> and <c>{task}</c>, then checks the
    /// result is inside <c>Assets/</c>.
    /// </summary>
    protected static string ResolveOutputFolder(HoudiniTask task, string sourceAssetPath)
    {
        var settings = HoudiniTaskSettingsLocator.Find();
        var pattern = string.IsNullOrWhiteSpace(task.outputFolderOverride)
            ? settings.outputPattern
            : task.outputFolderOverride;

        var sourceDir = Path.GetDirectoryName(sourceAssetPath)?.Replace('\\', '/') ?? "Assets";
        var name = Path.GetFileNameWithoutExtension(sourceAssetPath);

        var folder = pattern
            .Replace("{sourceDir}", sourceDir)
            .Replace("{name}", name)
            .Replace("{task}", task.TaskName)
            .Replace('\\', '/')
            .TrimEnd('/');

        if (!folder.StartsWith("Assets/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Output folder '{folder}' must be under Assets/. Check Output Pattern / Output Folder Override.");
        }

        return folder;
    }
}

/// <summary>Finds the single Houdini Task Settings asset, or explains why it can't.</summary>
public static class HoudiniTaskSettingsLocator
{
    public static HoudiniTaskSettings Find()
    {
        var guids = AssetDatabase.FindAssets("t:HoudiniTaskSettings");

        if (guids.Length == 0)
        {
            throw new InvalidOperationException(
                "No HoudiniTaskSettings asset in the project. Create one: " +
                "Assets > Create > Boomer > Houdini Task Settings.");
        }

        if (guids.Length > 1)
        {
            throw new InvalidOperationException(
                $"Found {guids.Length} HoudiniTaskSettings assets. Keep exactly one.");
        }

        return AssetDatabase.LoadAssetAtPath<HoudiniTaskSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }
}
