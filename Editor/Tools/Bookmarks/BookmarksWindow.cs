#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace stoogebag.Editor.Tools
{
    public class BookmarksWindow : EditorWindow
    {
        private BookmarkData _data;
        private int _selectedCategoryIndex;
        private Vector2 _scrollPos;
        private const string DataPath = "Assets/Assets/99_DevTools/EditorTools/BookmarkData.asset";

        private bool _isRenaming;
        private string _renameBuffer;

        private Dictionary<string, Object> _sceneResolveCache = new Dictionary<string, Object>();
        private bool _sceneCacheDirty = true;

        [MenuItem("Tools/Bookmarks %#b")]
        public static void ShowWindow()
        {
            var window = GetWindow<BookmarksWindow>("Bookmarks");
            window.minSize = new Vector2(280, 200);
        }

        private void OnEnable()
        {
            LoadOrCreateData();
            EditorSceneManager.sceneOpened      += OnSceneChanged;
            EditorSceneManager.sceneClosed      += OnSceneChanged;
            EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened      -= OnSceneChanged;
            EditorSceneManager.sceneClosed      -= OnSceneChanged;
            EditorSceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnSceneChanged(Scene scene, OpenSceneMode mode) => InvalidateSceneCache();
        private void OnSceneChanged(Scene scene) => InvalidateSceneCache();
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene) => InvalidateSceneCache();

        private void InvalidateSceneCache()
        {
            _sceneCacheDirty = true;
            Repaint();
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

            ResolveSceneBookmarks();

            DrawCategoryBar();
            DrawDropZone();
            DrawItems();
        }

        private void ResolveSceneBookmarks()
        {
            if (!_sceneCacheDirty) return;
            _sceneCacheDirty = false;

            var category = _data.categories[_selectedCategoryIndex];
            if (category == null || category.sceneItems.Count == 0)
            {
                _sceneResolveCache.Clear();
                return;
            }

            var gids = new List<GlobalObjectId>();
            foreach (var sb in category.sceneItems)
            {
                if (GlobalObjectId.TryParse(sb.globalObjectId, out var gid))
                    gids.Add(gid);
            }

            var resolved = new Object[gids.Count];
            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(gids.ToArray(), resolved);

            _sceneResolveCache.Clear();
            for (int i = 0; i < resolved.Length; i++)
            {
                if (resolved[i] != null)
                    _sceneResolveCache[gids[i].ToString()] = resolved[i];
            }
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
                    _sceneCacheDirty = true;
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
            GUI.Label(dropArea, "Drag assets or scene objects here", style);

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
                        HashSet<string> existingSceneGids = new HashSet<string>(
                            category.sceneItems.Select(s => s.globalObjectId)
                        );

                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);

                            if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets/"))
                            {
                                if (AssetDatabase.IsValidFolder(path))
                                    continue;
                                if (!existingPaths.Contains(path))
                                {
                                    category.items.Add(obj);
                                    existingPaths.Add(path);
                                }
                            }
                            else if (obj is GameObject go && go.scene != null && go.scene.IsValid())
                            {
                                var gid = GlobalObjectId.GetGlobalObjectIdSlow(go);
                                string gidStr = gid.ToString();
                                if (!existingSceneGids.Contains(gidStr))
                                {
                                    category.sceneItems.Add(new SceneBookmark
                                    {
                                        globalObjectId = gidStr,
                                        displayName    = go.name,
                                        scenePath       = go.scene.path
                                    });
                                    existingSceneGids.Add(gidStr);
                                }
                            }
                        }

                        InvalidateSceneCache();
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

            bool anyItems = category.items.Count > 0 || category.sceneItems.Count > 0;

            if (!anyItems)
            {
                EditorGUILayout.LabelField("No bookmarks yet. Drag assets or scene objects above.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < category.items.Count; i++)
                {
                    var obj = category.items[i];
                    DrawItemRow(obj, i, category);
                }

                if (category.sceneItems.Count > 0)
                    EditorGUILayout.Space(2);

                for (int i = 0; i < category.sceneItems.Count; i++)
                {
                    DrawSceneItemRow(category.sceneItems[i], i, category);
                }
            }

            EditorGUILayout.EndScrollView();

            if (anyItems)
            {
                EditorGUILayout.Space(2);
                if (GUILayout.Button("Clear All"))
                {
                    if (EditorUtility.DisplayDialog("Clear All",
                        $"Clear all bookmarks in '{category.name}'?", "Clear", "Cancel"))
                    {
                        category.items.Clear();
                        category.sceneItems.Clear();
                        InvalidateSceneCache();
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

        private void DrawSceneItemRow(SceneBookmark entry, int index, BookmarkCategory category)
        {
            EditorGUILayout.BeginHorizontal();

            Object resolved = null;
            _sceneResolveCache.TryGetValue(entry.globalObjectId, out resolved);

            var activeScene = EditorSceneManager.GetActiveScene();
            bool live = resolved is GameObject go
                && go.scene.IsValid()
                && go.scene.isLoaded
                && go.scene == activeScene;

            if (live)
            {
                var icon = AssetPreview.GetMiniThumbnail(resolved);
                GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

                if (GUILayout.Button(entry.displayName, EditorStyles.label))
                {
                    Selection.activeObject = resolved;
                    EditorGUIUtility.PingObject(resolved);
                }
            }
            else
            {
                GUI.color = new Color(0.55f, 0.55f, 0.55f, 0.7f);
                var icon = EditorGUIUtility.IconContent("GameObject Icon");
                GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    string sceneName = string.IsNullOrEmpty(entry.scenePath)
                        ? "unknown scene"
                        : Path.GetFileNameWithoutExtension(entry.scenePath);
                    GUILayout.Button($"{entry.displayName}  ({sceneName})", EditorStyles.label);
                }
                GUI.color = Color.white;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                category.sceneItems.RemoveAt(index);
                InvalidateSceneCache();
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