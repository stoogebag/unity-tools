using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NumpadMoverSettings", menuName = "Numpad Mover/Settings")]
public class NumpadMoverSettings : ScriptableObject
{
    public float moveFull = 1f;
    public float moveSmall = 0.5f;
    public float rotate90 = 90f;
    public float rotateSmall = 45f;

    public AxisChoice Axis;
    
    public enum AxisChoice
    {
        XY,
        XZ,
    }
    
    internal const string Folder = "Assets/Settings/Resources";
    internal const string AssetPath = Folder + "/NumpadMoverSettings.asset";

    public static NumpadMoverSettings Instance => Load();
    static NumpadMoverSettings Load()
    {
        var settings = Resources.Load<NumpadMoverSettings>("NumpadMoverSettings");
        if (settings == null)
        {
            settings = CreateInstance<NumpadMoverSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets/Settings", "Resources");
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.Refresh();
        }
        return settings;
    }
}

static class NumpadMoverSettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider("Project/stooge/Numpad Mover", SettingsScope.Project);
        provider.guiHandler = search =>
        {
            var settings = AssetDatabase.LoadAssetAtPath<NumpadMoverSettings>(NumpadMoverSettings.AssetPath);
            if (settings == null) return;

            var so = new SerializedObject(settings);
            so.Update();
            EditorGUILayout.PropertyField(so.FindProperty("moveFull"));
            EditorGUILayout.PropertyField(so.FindProperty("moveSmall"));
            EditorGUILayout.PropertyField(so.FindProperty("rotate90"));
            EditorGUILayout.PropertyField(so.FindProperty("rotateSmall"));
            EditorGUILayout.PropertyField(so.FindProperty("Axis"));
            so.ApplyModifiedProperties();
        };
        return provider;
    }
}