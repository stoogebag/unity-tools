#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Recordable.Editor
{
    [CustomPropertyDrawer(typeof(RecordableAttribute))]
    public class RecordablePropertyDrawer : PropertyDrawer
    {
        private const float Gap = 4f;

        private static readonly Dictionary<string, AudioClip> _tempClips = new();
        private static readonly Dictionary<string, bool> _recording = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (fieldInfo.FieldType != typeof(AudioClip))
            {
                EditorGUI.HelpBox(position, "[Recordable] can only be used on AudioClip fields.", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            var attr = (RecordableAttribute)attribute;
            var path = property.propertyPath;
            var line = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var half = position.width / 2f - Gap / 2f;

            var clipRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property, label, false));
            EditorGUI.PropertyField(clipRect, property, label);

            var y = clipRect.yMax + spacing;
            bool recording = _recording.TryGetValue(path, out var r) && r;

            // Record / Stop
            using (new EditorGUI.DisabledGroupScope(recording))
            {
                if (GUI.Button(new Rect(position.x, y, half, line), recording ? "Recording..." : "Record"))
                {
                    var clip = RecordableAudio.Start(RecordableAudio.GetActiveDevice(), attr.MaxLengthSeconds);
                    if (clip != null) { _tempClips[path] = clip; _recording[path] = true; }
                }
            }
            using (new EditorGUI.DisabledGroupScope(!recording))
            {
                if (GUI.Button(new Rect(position.x + half + Gap, y, half, line), "Stop"))
                {
                    RecordableAudio.Stop(RecordableAudio.GetActiveDevice());
                    _recording[path] = false;
                }
            }
            y += line + spacing;

            // Play / Save
            using (new EditorGUI.DisabledGroupScope(recording))
            {
                if (GUI.Button(new Rect(position.x, y, half, line), "Play"))
                {
                    var clip = property.objectReferenceValue as AudioClip;
                    if (clip == null) _tempClips.TryGetValue(path, out clip);
                    RecordableAudio.Play(clip);
                }

                if (GUI.Button(new Rect(position.x + half + Gap, y, half, line), "Save"))
                {
                    if (_tempClips.TryGetValue(path, out var temp) && temp != null)
                    {
                        RecordableAudio.Stop(RecordableAudio.GetActiveDevice());
                        property.objectReferenceValue = RecordableAudio.Save(temp, GetSaveBasePath(property, attr));
                        property.serializedObject.ApplyModifiedProperties();
                        _tempClips.Remove(path);
                        _recording[path] = false;
                    }
                }
            }
            y += line + spacing;

            if (!string.IsNullOrEmpty(attr.TranscribeIntoField))
            {
                using (new EditorGUI.DisabledGroupScope(recording))
                {
                    if (GUI.Button(new Rect(position.x, y, position.width, line), "Transcribe"))
                    {
                        var clip = property.objectReferenceValue as AudioClip;
                        if (clip == null) _tempClips.TryGetValue(path, out clip);
                        TranscribeInto(property, clip, attr.TranscribeIntoField);
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (RecordableAttribute)attribute;
            var rows = string.IsNullOrEmpty(attr.TranscribeIntoField) ? 2 : 3;
            return EditorGUI.GetPropertyHeight(property, label, false)
                + rows * EditorGUIUtility.singleLineHeight
                + (rows + 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        private static async void TranscribeInto(SerializedProperty audioProp, AudioClip clip, string textField)
        {
            var text = await RecordableAudio.Transcribe(clip);
            if (string.IsNullOrEmpty(text)) return;

            var target = FindSiblingProperty(audioProp, textField);
            if (target != null)
            {
                target.stringValue = text;
                target.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"[Recordable] Transcribe target field '{textField}' not found.");
            }
        }

        private string GetSaveBasePath(SerializedProperty audioProp, RecordableAttribute attr)
        {
            var host = GetPropertyHost(audioProp);
            if (host is IRecordablePathProvider provider)
            {
                var customPath = provider.GetRecordableSavePath(audioProp.name);
                if (!string.IsNullOrWhiteSpace(customPath))
                    return SanitizePath(customPath);
            }

            var folder = attr.SaveFolder;
            var subFolder = GetSiblingStringValue(audioProp, attr.SaveFolderField);
            if (!string.IsNullOrWhiteSpace(subFolder))
                folder = $"{folder}/{subFolder}";

            var fileBase = attr.FileNamePrefix;
            var fileFromField = GetSiblingStringValue(audioProp, attr.FileNameField);
            if (!string.IsNullOrWhiteSpace(fileFromField))
                fileBase = fileFromField;

            return SanitizePath($"{folder}/{fileBase}");
        }

        private static SerializedProperty FindSiblingProperty(SerializedProperty property, string siblingFieldName)
        {
            if (string.IsNullOrEmpty(siblingFieldName)) return null;

            var path = property.propertyPath;
            var lastDot = path.LastIndexOf('.');
            var parentPath = lastDot < 0 ? "" : path.Substring(0, lastDot);
            var siblingPath = string.IsNullOrEmpty(parentPath)
                ? siblingFieldName
                : $"{parentPath}.{siblingFieldName}";

            return property.serializedObject.FindProperty(siblingPath);
        }

        private static string GetSiblingStringValue(SerializedProperty property, string siblingFieldName)
        {
            var sibling = FindSiblingProperty(property, siblingFieldName);
            if (sibling != null && sibling.propertyType == SerializedPropertyType.String)
                return sibling.stringValue;

            return null;
        }

        private static object GetPropertyHost(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            var parts = property.propertyPath.Split('.');
            object current = target;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (current == null) return null;

                var part = parts[i];
                if (part == "Array" && i + 1 < parts.Length - 1)
                {
                    var indexPart = parts[i + 1];
                    if (indexPart.Length <= 6 || !indexPart.StartsWith("data[") || !indexPart.EndsWith("]"))
                        return null;

                    var indexString = indexPart.Substring(5, indexPart.Length - 6);
                    if (!int.TryParse(indexString, out var index))
                        return null;

                    if (current is IList list && index >= 0 && index < list.Count)
                        current = list[index];
                    else
                        return null;

                    i++;
                }
                else
                {
                    var field = GetField(current.GetType(), part);
                    if (field == null) return null;
                    current = field.GetValue(current);
                }
            }

            return current;
        }

        private static FieldInfo GetField(Type type, string name)
        {
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

        private static string SanitizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            var segments = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
                segments[i] = SanitizeSegment(segments[i]);
            return string.Join("/", segments);
        }

        private static string SanitizeSegment(string segment)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                segment = segment.Replace(c, '_');
            return segment.Trim();
        }
    }
}
#endif
