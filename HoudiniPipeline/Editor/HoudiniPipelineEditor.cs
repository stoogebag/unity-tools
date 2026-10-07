using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(USDModelImporter))]
public class HoudiniPipelineEditor : Editor
{
    SerializedProperty houdiniProject;
    SerializedProperty houdiniNodePath;
    SerializedProperty hythonPath;
    SerializedProperty processScriptPath;
    SerializedProperty pipelineRootFolder;
    SerializedProperty usdOutputFolder;
    SerializedProperty usdExportFolder;
    SerializedProperty disableSourceOnImport;

    void OnEnable()
    {
        houdiniProject = serializedObject.FindProperty("houdiniProject");
        houdiniNodePath = serializedObject.FindProperty("houdiniNodePath");
        hythonPath = serializedObject.FindProperty("hythonPath");
        processScriptPath = serializedObject.FindProperty("processScriptPath");
        pipelineRootFolder = serializedObject.FindProperty("pipelineRootFolder");
        usdOutputFolder = serializedObject.FindProperty("usdOutputFolder");
        usdExportFolder = serializedObject.FindProperty("usdExportFolder");
        disableSourceOnImport = serializedObject.FindProperty("disableSourceOnImport");
    }

    public override void OnInspectorGUI()
    {
        var importer = (USDModelImporter)target;

        serializedObject.Update();

        EditorGUILayout.LabelField("Houdini", EditorStyles.boldLabel);
        DrawPath(houdiniProject, "Houdini Project", "hip,hiplc", false);
        EditorGUILayout.PropertyField(houdiniNodePath, new GUIContent("Houdini Node Path"));
        DrawPath(hythonPath, "Hython", "exe", false);
        DrawPath(processScriptPath, "Process Script", "py", false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pipeline Folders", EditorStyles.boldLabel);
        DrawPath(pipelineRootFolder, "Pipeline Root", null, true);
        DrawPath(usdOutputFolder, "USD Output", null, true);
        DrawPath(usdExportFolder, "USD Export", null, true);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(disableSourceOnImport, new GUIContent("Disable Source On Import"));

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Export USD"))
            importer.ExportToDefaultFolder();

        if (GUILayout.Button("Import Last Output"))
            importer.ImportDefault();

        if (GUILayout.Button("Export + Process"))
            importer.RunPipeline();
    }

    void DrawPath(SerializedProperty prop, string label, string extension, bool folder)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(prop, new GUIContent(label));
        if (GUILayout.Button("…", GUILayout.Width(24)))
        {
            string start = string.IsNullOrEmpty(prop.stringValue)
                ? string.Empty
                : Path.GetDirectoryName(prop.stringValue);

            string picked = folder
                ? EditorUtility.OpenFolderPanel(label, start, string.Empty)
                : EditorUtility.OpenFilePanel(label, start, extension);

            if (!string.IsNullOrEmpty(picked))
                prop.stringValue = ToProjectRelative(picked);
        }
        EditorGUILayout.EndHorizontal();
    }

    static string ToProjectRelative(string absolute)
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath).Replace('\\', '/');
        string normalized = absolute.Replace('\\', '/');

        if (normalized.StartsWith(projectRoot + "/", StringComparison.Ordinal))
            return normalized.Substring(projectRoot.Length + 1);

        int assetsIdx = normalized.IndexOf("/Assets/", StringComparison.Ordinal);
        if (assetsIdx >= 0)
            return normalized.Substring(assetsIdx + 1);

        return normalized;
    }
}
