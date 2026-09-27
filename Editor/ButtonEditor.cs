using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class ButtonEditor : Editor
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static
                             | BindingFlags.Public | BindingFlags.NonPublic;

    static readonly Dictionary<MethodInfo, GUIContent> Contents = new();

    public override void OnInspectorGUI()
    {
        DrawButtons();
        DrawDefaultInspector();
    }

    void DrawButtons()
    {
        var entries = Entries(target.GetType());

        for (var i = 0; i < entries.Count;)
        {
            var end = i + 1;
            if (entries[i].Grouped)
                while (end < entries.Count && entries[end].Grouped && entries[end].Group == entries[i].Group)
                    end++;

            if (end - i == 1)
            {
                DrawButton(entries[i]);
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                    for (var j = i; j < end; j++)
                        DrawButton(entries[j], true);
            }

            i = end;
        }
    }

    void DrawButton(Entry entry, bool inRow = false)
    {
        var options = inRow ? Expand : Array.Empty<GUILayoutOption>();
        if (!GUILayout.Button(Content(entry.Method, entry.Button), options)) return;

        Undo.RecordObjects(targets, entry.Method.Name);
        try { entry.Method.Invoke(entry.Method.IsStatic ? null : target, null); }
        catch (Exception e) { Debug.LogException(e); }
        foreach (var t in targets) EditorUtility.SetDirty(t);
        Repaint();
    }

    static readonly GUILayoutOption[] Expand = { GUILayout.ExpandWidth(true) };

    static List<Entry> Entries(Type type)
    {
        var entries = new List<Entry>();
        foreach (var method in type.GetMethods(Flags))
        {
            var button = method.GetCustomAttribute<ButtonAttribute>();
            if (button == null || method.GetParameters().Length > 0) continue;

            var group = method.GetCustomAttribute<ButtonGroupAttribute>();
            entries.Add(new Entry
            {
                Method = method,
                Button = button,
                Grouped = group != null,
                Group = group?.Name,
            });
        }

        entries.Sort((a, b) => a.Method.MetadataToken.CompareTo(b.Method.MetadataToken));
        return entries;
    }

    class Entry
    {
        public MethodInfo Method;
        public ButtonAttribute Button;
        public bool Grouped;
        public string Group;
    }

    static GUIContent Content(MethodInfo method, ButtonAttribute attr)
    {
        if (Contents.TryGetValue(method, out var cached)) return cached;

        var label = attr.Label ?? ObjectNames.NicifyVariableName(method.Name);
        var icon = Icon(attr.Icon);

        var content = icon != null ? new GUIContent(label, icon) : new GUIContent(label);
        Contents[method] = content;
        return content;
    }

    static Texture2D Icon(UnityIcon icon)
    {
        var name = IconName(icon);
        if (name == null) return null;
        return Image(EditorGUIUtility.isProSkin ? "d_" + name : name) ?? Image(name);
    }

    static Texture2D Image(string name)
    {
        var content = EditorGUIUtility.IconContent(name);
        return content != null ? content.image as Texture2D : null;
    }

    static string IconName(UnityIcon icon) => icon switch
    {
        UnityIcon.Play => "PlayButton",
        UnityIcon.Pause => "PauseButton",
        UnityIcon.Step => "StepButton",
        UnityIcon.Previous => "PrevButton",
        UnityIcon.Next => "NextButton",
        UnityIcon.Record => "Animation.Record",
        UnityIcon.Refresh => "Refresh",
        UnityIcon.Add => "Toolbar Plus",
        UnityIcon.Remove => "Toolbar Minus",
        UnityIcon.Edit => "editicon.sml",
        UnityIcon.Settings => "Settings",
        UnityIcon.Delete => "TreeEditor.Trash",
        UnityIcon.Save => "SaveAs",
        _ => null,
    };
}