#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using JD.EditorAudioUtils;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Dialogue.Editor
{
    [CustomPropertyDrawer(typeof(RecordableAttribute))]
    public class RecordablePropertyDrawer : PropertyDrawer
    {
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

            var clipRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property, label, false));
            EditorGUI.PropertyField(clipRect, property, label);

            var controlsY = clipRect.yMax + EditorGUIUtility.standardVerticalSpacing;
            var controlsRect = new Rect(position.x, controlsY, position.width, position.yMax - controlsY);
            DrawRecordingControls(controlsRect, property, path, attr);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = EditorGUI.GetPropertyHeight(property, label, false);
            return baseHeight
                + EditorGUIUtility.singleLineHeight * 2f
                + EditorGUIUtility.standardVerticalSpacing * 2f;
        }

        private void DrawRecordingControls(Rect rect, SerializedProperty audioProp, string path, RecordableAttribute attr)
        {
            bool recording = _recording.TryGetValue(path, out var rec) && rec;
            var halfWidth = rect.width / 2f - 2f;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            // Record / Stop row
            var recordRect = new Rect(rect.x, rect.y, halfWidth, lineHeight);
            var stopRect = new Rect(rect.x + halfWidth + 4f, rect.y, halfWidth, lineHeight);

            using (new EditorGUI.DisabledGroupScope(recording))
            {
                if (GUI.Button(recordRect, recording ? "Recording..." : "Record"))
                {
                    var device = RecordingSettings.GetActiveDevice();
                    if (!string.IsNullOrEmpty(device))
                    {
                        _tempClips[path] = Microphone.Start(device, false, attr.MaxLengthSeconds, 44100);
                        _recording[path] = true;
                    }
                }
            }

            using (new EditorGUI.DisabledGroupScope(!recording))
            {
                if (GUI.Button(stopRect, "Stop"))
                {
                    var device = RecordingSettings.GetActiveDevice();
                    if (!string.IsNullOrEmpty(device))
                        Microphone.End(device);
                    _recording[path] = false;
                }
            }

            rect.y += lineHeight + spacing;

            // Play / Save row
            var playRect = new Rect(rect.x, rect.y, halfWidth, lineHeight);
            var saveRect = new Rect(rect.x + halfWidth + 4f, rect.y, halfWidth, lineHeight);

            using (new EditorGUI.DisabledGroupScope(recording))
            {
                if (GUI.Button(playRect, "Play"))
                {
                    var clip = audioProp.objectReferenceValue as AudioClip;
                    if (clip != null)
                    {
                        EditorAudioUtility.PlayPreviewClip(clip);
                    }
                    else if (_tempClips.TryGetValue(path, out var tempClip) && tempClip != null)
                    {
                        EditorAudioUtility.PlayPreviewClip(tempClip);
                    }
                }

                if (GUI.Button(saveRect, "Save"))
                {
                    if (_tempClips.TryGetValue(path, out var tempClip) && tempClip != null)
                    {
                        SaveClip(audioProp, tempClip, path, attr);
                        _recording[path] = false;
                    }
                }
            }
        }

        private void SaveClip(SerializedProperty audioProp, AudioClip tempClip, string path, RecordableAttribute attr)
        {
            var device = RecordingSettings.GetActiveDevice();
            if (!string.IsNullOrEmpty(device))
                Microphone.End(device);

            var basePath = GetSaveBasePath(audioProp, attr);
            var guid = Guid.NewGuid();
            var wavPath = $"{basePath}-{guid}";

            var trimmed = SavWav.TrimSilence(tempClip, 0.001f);
            SavWav.Save(wavPath, trimmed);

            Thread.Sleep(10);
            AssetDatabase.ImportAsset("Assets/" + wavPath + ".wav", ImportAssetOptions.ForceSynchronousImport);
            Thread.Sleep(10);

            var savedClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/" + wavPath + ".wav");
            audioProp.objectReferenceValue = savedClip;
            audioProp.serializedObject.ApplyModifiedProperties();

            _tempClips.Remove(path);
        }

        private string GetSaveBasePath(SerializedProperty audioProp, RecordableAttribute attr)
        {
            // 1. Programmatic path provider on the host object
            var host = GetPropertyHost(audioProp);
            if (host is IRecordablePathProvider provider)
            {
                var customPath = provider.GetRecordableSavePath(audioProp.name);
                if (!string.IsNullOrWhiteSpace(customPath))
                    return SanitizePath(customPath);
            }

            // 2. Sibling field references
            var folder = attr.SaveFolder;
            var subFolder = GetSiblingStringValue(audioProp, attr.SaveFolderField);
            if (!string.IsNullOrWhiteSpace(subFolder))
                folder = $"{folder}/{subFolder}";

            var fileBase = attr.FileNamePrefix;
            var fileFromField = GetSiblingStringValue(audioProp, attr.FileNameField);
            if (!string.IsNullOrWhiteSpace(fileFromField))
                fileBase = fileFromField;

            // 3. Static attribute fallback
            return SanitizePath($"{folder}/{fileBase}");
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

        private static string GetSiblingStringValue(SerializedProperty property, string siblingFieldName)
        {
            if (string.IsNullOrEmpty(siblingFieldName)) return null;

            var path = property.propertyPath;
            var lastDot = path.LastIndexOf('.');
            var parentPath = lastDot < 0 ? "" : path.Substring(0, lastDot);
            var siblingPath = string.IsNullOrEmpty(parentPath)
                ? siblingFieldName
                : $"{parentPath}.{siblingFieldName}";

            var sibling = property.serializedObject.FindProperty(siblingPath);
            if (sibling != null && sibling.propertyType == SerializedPropertyType.String)
                return sibling.stringValue;

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
