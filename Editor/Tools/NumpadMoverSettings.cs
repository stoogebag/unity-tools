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
    
    public static NumpadMoverSettings Instance => Load();
    static NumpadMoverSettings Load() => Resources.Load<NumpadMoverSettings>("NumpadMoverSettings");
}

static class NumpadMoverSettingsProvider
{
    const string PATH = "Assets/Resources/NumpadMoverSettings.asset";

    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider("Project/stooge/Numpad Mover", SettingsScope.Project);
        provider.guiHandler = search =>
        {
            var settings = AssetDatabase.LoadAssetAtPath<NumpadMoverSettings>(PATH);
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