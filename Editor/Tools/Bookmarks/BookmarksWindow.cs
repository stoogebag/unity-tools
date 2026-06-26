#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace stoogebag.Editor.Tools
{
    public class BookmarksWindow : EditorWindow
    {
        private BookmarkData _data;
        private int _selectedCategoryIndex;
        private Vector2 _scrollPos;
        private const string DataPath = "Assets/stoogebag/Editor/Tools/Bookmarks/BookmarkData.asset";

        private bool _isRenaming;
        private string _renameBuffer;

        [MenuItem("Tools/Bookmarks %#b")]
        public static void ShowWindow()
        {
            var window = GetWindow<BookmarksWindow>("Bookmarks");
            window.minSize = new Vector2(280, 200);
        }

        private void OnEnable()
        {
            LoadOrCreateData();
        }

        private void LoadOrCreateData()
        {
            _data = AssetDatabase.LoadAssetAtPath<BookmarkData>(DataPath);

            if (_data == null)
            {
                _data = CreateInstance<BookmarkData>();
                _data.categories.Add(new BookmarkCategory { name = "Default" });

                string dir = Path.GetDirectoryName(DataPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                AssetDatabase.CreateAsset(_data, DataPath);
                AssetDatabase.SaveAssets();
            }

            if (_data.categories.Count == 0)
            {
                _data.categories.Add(new BookmarkCategory { name = "Default" });
                EditorUtility.SetDirty(_data);
            }

            _selectedCategoryIndex = Mathf.Clamp(_selectedCategoryIndex, 0, _data.categories.Count - 1);
        }

        private void OnGUI()
        {
            if (_data == null)
            {
                LoadOrCreateData();
                if (_data == null) return;
            }

            DrawCategoryBar();
            DrawDropZone();
            DrawItems();
        }

        private void DrawCategoryBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (_isRenaming)
            {
                GUI.SetNextControlName("renameField");
                _renameBuffer = EditorGUILayout.TextField(_renameBuffer, GUILayout.Width(150));

                if (GUILayout.Button("OK", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    ApplyRename();
            }
            else
            {
                string[] names = _data.categories.Select(c => c.name).ToArray();
                int selected = EditorGUILayout.Popup(_selectedCategoryIndex, names, EditorStyles.toolbarPopup, GUILayout.Width(150));
                if (selected != _selectedCategoryIndex)
                {
                    _selectedCategoryIndex = selected;
                    _scrollPos = Vector2.zero;
                }

                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    string name = "Category " + (_data.categories.Count + 1);
                    _data.categories.Add(new BookmarkCategory { name = name });
                    _selectedCategoryIndex = _data.categories.Count - 1;
                    MarkDirty();
                }

                if (GUILayout.Button("Rename", EditorStyles.toolbarButton, GUILayout.Width(56)))
                {
                    _isRenaming = true;
                    _renameBuffer = _data.categories[_selectedCategoryIndex].name;
                    EditorGUI.FocusTextInControl("renameField");
                }
            }

            GUILayout.FlexibleSpace();

            bool canDelete = _data.categories.Count > 1;
            EditorGUI.BeginDisabledGroup(!canDelete);
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(24)))
            {
                var cat = _data.categories[_selectedCategoryIndex];
                if (EditorUtility.DisplayDialog("Delete Category",
                    $"Delete '{cat.name}' and all its bookmarks?", "Delete", "Cancel"))
                {
                    _data.categories.RemoveAt(_selectedCategoryIndex);
                    _selectedCategoryIndex = Mathf.Clamp(_selectedCategoryIndex, 0, _data.categories.Count - 1);
                    MarkDirty();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (_isRenaming && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                ApplyRename();
                Event.current.Use();
                Repaint();
            }
        }

        private void ApplyRename()
        {
            if (!string.IsNullOrEmpty(_renameBuffer))
            {
                _data.categories[_selectedCategoryIndex].name = _renameBuffer;
                MarkDirty();
            }
            _isRenaming = false;
            GUI.FocusControl(null);
        }

        private void DrawDropZone()
        {
            var evt = Event.current;
            var dropArea = GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true));

            bool hover = dropArea.Contains(evt.mousePosition);
            Color bgColor = hover ? new Color(0.3f, 0.3f, 0.3f, 0.5f) : new Color(0.18f, 0.18f, 0.18f, 0.4f);
            EditorGUI.DrawRect(dropArea, bgColor);

            var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(dropArea, "Drag assets here", style);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        var category = _data.categories[_selectedCategoryIndex];

                        HashSet<string> existingPaths = new HashSet<string>(
                            category.items.Where(o => o != null).Select(AssetDatabase.GetAssetPath)
                        );

                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);
                            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/"))
                                continue;
                            if (AssetDatabase.IsValidFolder(path))
                                continue;
                            if (!existingPaths.Contains(path))
                            {
                                category.items.Add(obj);
                                existingPaths.Add(path);
                            }
                        }

                        MarkDirty();
                    }
                    break;
            }
        }

        private void DrawItems()
        {
            var category = _data.categories[_selectedCategoryIndex];
            if (category == null) return;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (category.items.Count == 0)
            {
                EditorGUILayout.LabelField("No bookmarks yet. Drag assets above.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < category.items.Count; i++)
                {
                    var obj = category.items[i];
                    DrawItemRow(obj, i, category);
                }
            }

            EditorGUILayout.EndScrollView();

            if (category.items.Count > 0)
            {
                EditorGUILayout.Space(2);
                if (GUILayout.Button("Clear All"))
                {
                    if (EditorUtility.DisplayDialog("Clear All",
                        $"Clear all bookmarks in '{category.name}'?", "Clear", "Cancel"))
                    {
                        category.items.Clear();
                        MarkDirty();
                    }
                }
            }
        }

        private void DrawItemRow(Object obj, int index, BookmarkCategory category)
        {
            EditorGUILayout.BeginHorizontal();

            var icon = AssetPreview.GetMiniThumbnail(obj);
            GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

            if (obj != null)
            {
                if (GUILayout.Button(obj.name, EditorStyles.label))
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("(Missing)", EditorStyles.label);
                GUI.color = Color.white;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                category.items.RemoveAt(index);
                MarkDirty();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void MarkDirty()
        {
            EditorUtility.SetDirty(_data);
            Repaint();
        }

        private void OnProjectChange()
        {
            Repaint();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }
    }
}
#endif
