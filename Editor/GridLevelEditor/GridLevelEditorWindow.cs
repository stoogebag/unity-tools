#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Editor
{
    public class GridLevelEditorWindow : EditorWindow
    {
        private const float GridSpacing = 10f;

        [SerializeField] private List<GameObject> _prefabSlots = new List<GameObject>();
        private int _selectedSlot = -1;
        private Vector2 _scrollPos;

        private GameObject _placementGhost;
        private GameObject _ghostPrefab;
        private Vector3 _ghostPosition;
        private bool _showGhost;
        private static Material _ghostMaterial;

        private float _gridY;
        private bool _active = true;

        [MenuItem("stooge/Grid Level Editor")]
        public static void ShowWindow()
        {
            var w = GetWindow<GridLevelEditorWindow>();
            w.titleContent = new GUIContent("Grid Level Editor");
            w.minSize = new Vector2(280, 400);
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            Undo.undoRedoPerformed -= Repaint;
            DestroyGhost();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
            Undo.undoRedoPerformed -= Repaint;
            DestroyGhost();
        }

        private void OnGUI()
        {
            var evt = Event.current;
            if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Tab)
            {
                ToggleActive();
                evt.Use();
                Repaint();
            }

            DrawToolbar();
            DrawPrefabSlots();
            DrawFooter();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Prefab Palette", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                _prefabSlots.Clear();
                _selectedSlot = -1;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPrefabSlots()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            for (var i = 0; i < _prefabSlots.Count; i++)
            {
                DrawPrefabSlot(i);
            }

            var dropRect = EditorGUILayout.BeginVertical(GUILayout.Height(80));
            {
                var bg = new Color(0.25f, 0.25f, 0.25f, 0.5f);
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 60), bg);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Drag Prefabs Here", EditorStyles.centeredGreyMiniLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();

            HandleDragDrop(dropRect);

            EditorGUILayout.EndScrollView();
        }

        private void DrawPrefabSlot(int index)
        {
            var prefab = _prefabSlots[index];
            EditorGUILayout.BeginHorizontal();

            var isSelected = _selectedSlot == index;
            var bgColor = isSelected ? new Color(0.3f, 0.5f, 0.8f, 0.5f) : Color.clear;

            var rect = EditorGUILayout.GetControlRect(false, 48);
            EditorGUI.DrawRect(rect, bgColor);

            var previewRect = new Rect(rect.x + 2, rect.y + 2, 44, 44);
            var tex = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
            if (tex != null)
            {
                GUI.DrawTexture(previewRect, tex, ScaleMode.ScaleToFit);
            }

            var labelRect = new Rect(rect.x + 50, rect.y, rect.width - 60, rect.height);
            var label = prefab != null ? prefab.name : "<missing>";
            EditorGUI.LabelField(labelRect, label, isSelected ? EditorStyles.whiteLabel : EditorStyles.label);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                var changed = isSelected ? (_selectedSlot != -1) : (_selectedSlot != index);
                _selectedSlot = isSelected ? -1 : index;
                if (changed) DestroyGhost();
                Event.current.Use();
                Repaint();
                SceneView.RepaintAll();
            }

            var btnRect = new Rect(rect.xMax - 18, rect.y + 2, 16, 16);
            if (GUI.Button(btnRect, "x", EditorStyles.miniButton))
            {
                _prefabSlots.RemoveAt(index);
                if (_selectedSlot == index) _selectedSlot = -1;
                else if (_selectedSlot > index) _selectedSlot--;
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void HandleDragDrop(Rect dropRect)
        {
            var evt = Event.current;
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform) return;
            if (!dropRect.Contains(evt.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    var go = obj as GameObject;
                    if (go != null)
                    {
                        _prefabSlots.Add(go);
                    }
                }
                _selectedSlot = _prefabSlots.Count - 1;
                Repaint();
            }

            evt.Use();
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Grid Y: ", GUILayout.Width(44));
            _gridY = EditorGUILayout.FloatField(_gridY, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            var activeLabel = _active ? "Active" : "Off";
            var activeColor = _active ? Color.green : Color.gray;
            var prevColor = GUI.color;
            GUI.color = activeColor;
            GUILayout.Label(activeLabel, EditorStyles.boldLabel);
            GUI.color = prevColor;
            GUILayout.Space(4);
            GUILayout.Label($"{_prefabSlots.Count} prefab(s)");
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneViewGUI(SceneView sceneView)
        {
            var evt = Event.current;
            var controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Tab)
            {
                ToggleActive();
                evt.Use();
                Repaint();
            }

            if (_active)
            {
                if (evt.type == EventType.Layout)
                    HandleUtility.AddDefaultControl(controlId);

                if (evt.type == EventType.KeyDown)
                {
                    if (evt.keyCode == KeyCode.PageUp)   { _gridY += GridSpacing; evt.Use(); }
                    if (evt.keyCode == KeyCode.PageDown)  { _gridY -= GridSpacing; evt.Use(); }
                }

                if (evt.type == EventType.MouseDown && evt.button == 1)
                {
                    var hit = HandleUtility.PickGameObject(evt.mousePosition, false);
                    if (hit != null)
                    {
                        var root = hit.transform.root.gameObject;
                        var source = PrefabUtility.GetCorrespondingObjectFromSource(root);
                        if (source != null && _prefabSlots.Contains(source))
                        {
                            Undo.DestroyObjectImmediate(root);
                            evt.Use();
                        }
                    }
                }

                DrawGridOverlay(sceneView);

                if (_selectedSlot >= 0 && _selectedSlot < _prefabSlots.Count)
                {
                    var prefab = _prefabSlots[_selectedSlot];
                    if (prefab != null) HandlePlacement(sceneView, prefab, evt, controlId);
                }
            }
        }

        private void HandlePlacement(SceneView sceneView, GameObject prefab, Event evt, int controlId)
        {
            var ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
            var plane = new Plane(Vector3.up, new Vector3(0, _gridY, 0));

            Vector3? snappedPos = null;
            if (plane.Raycast(ray, out var dist))
                snappedPos = SnapToGrid(ray.GetPoint(dist));

            if (snappedPos.HasValue)
            {
                DrawPlacementGhost(prefab, snappedPos.Value);

                if (evt.type == EventType.MouseDown && evt.button == 0 && !evt.alt)
                {
                    GUIUtility.hotControl = controlId;
                    PlacePrefab(prefab, snappedPos.Value);
                    evt.Use();
                }
            }

            if (evt.type == EventType.MouseMove)
                evt.Use();
        }

        private void DrawGridOverlay(SceneView sceneView)
        {
            var cam = sceneView.camera;
            var camPos = cam.transform.position;

            var sceneRect = sceneView.position;

            var worldSize = cam.ScreenToWorldPoint(new Vector3(sceneRect.width, sceneRect.height, cam.farClipPlane * 0.5f));
            var worldOrigin = cam.ScreenToWorldPoint(Vector3.zero);

            var halfSize = Mathf.Max(Mathf.Abs(worldSize.x - worldOrigin.x), Mathf.Abs(worldSize.z - worldOrigin.z)) * 1.5f;

            var centeredX = Mathf.Round(camPos.x / GridSpacing) * GridSpacing;
            var centeredZ = Mathf.Round(camPos.z / GridSpacing) * GridSpacing;

            var startX = centeredX - halfSize;
            var startZ = centeredZ - halfSize;
            var endX = centeredX + halfSize;
            var endZ = centeredZ + halfSize;

            Handles.color = new Color(1, 1, 1, 0.08f);
            for (var x = startX; x <= endX; x += GridSpacing)
            {
                Handles.DrawLine(new Vector3(x, _gridY, startZ), new Vector3(x, _gridY, endZ));
            }
            for (var z = startZ; z <= endZ; z += GridSpacing)
            {
                Handles.DrawLine(new Vector3(startX, _gridY, z), new Vector3(endX, _gridY, z));
            }

            Handles.color = new Color(1, 1, 1, 0.15f);
            Handles.DrawLine(new Vector3(0, _gridY, startZ), new Vector3(0, _gridY, endZ));
            Handles.DrawLine(new Vector3(startX, _gridY, 0), new Vector3(endX, _gridY, 0));

            Handles.color = new Color(1, 1, 1, 0.25f);
            Handles.DrawLine(new Vector3(0, _gridY, -0.5f), new Vector3(0, _gridY, 0.5f));
            Handles.DrawLine(new Vector3(-0.5f, _gridY, 0), new Vector3(0.5f, _gridY, 0));
        }

        private Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / GridSpacing) * GridSpacing,
                _gridY,
                Mathf.Round(pos.z / GridSpacing) * GridSpacing
            );
        }

        private void DrawPlacementGhost(GameObject prefab, Vector3 position)
        {
            if (_placementGhost == null || _ghostPrefab != prefab)
            {
                DestroyGhost();
                _placementGhost = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                _placementGhost.name = "PLACEMENT_GHOST";
                _placementGhost.hideFlags = HideFlags.HideAndDontSave;
                OverrideGhostRenderers(_placementGhost);
                _ghostPrefab = prefab;
            }

            _placementGhost.transform.position = position;
            _showGhost = true;
        }

        private static Material GetGhostMaterial()
        {
            if (_ghostMaterial == null)
            {
                _ghostMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                _ghostMaterial.color = new Color(0.3f, 0.7f, 1f, 0.4f);
                _ghostMaterial.SetFloat("_Surface", 1);
                _ghostMaterial.SetFloat("_Blend", 0);
                _ghostMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _ghostMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _ghostMaterial.SetFloat("_ZWrite", 0);
                _ghostMaterial.SetInt("_AlphaClip", 0);
                _ghostMaterial.renderQueue = 3000;
                _ghostMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _ghostMaterial;
        }

        private static void OverrideGhostRenderers(GameObject ghost)
        {
            var renderers = ghost.GetComponentsInChildren<Renderer>();
            var ghostMat = GetGhostMaterial();
            foreach (var r in renderers)
            {
                var mats = r.sharedMaterials;
                for (var i = 0; i < mats.Length; i++)
                    mats[i] = ghostMat;
                r.sharedMaterials = mats;
            }

            var colliders = ghost.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = false;
        }

        private void PlacePrefab(GameObject prefab, Vector3 position)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (go == null) return;
            go.transform.position = position;
            Undo.RegisterCreatedObjectUndo(go, $"Place {prefab.name}");
            Selection.activeGameObject = go;
        }

        private void ToggleActive()
        {
            _active = !_active;
            if (!_active) DestroyGhost();
        }

        private void DestroyGhost()
        {
            if (_placementGhost != null)
            {
                DestroyImmediate(_placementGhost);
                _placementGhost = null;
            }
            _showGhost = false;
        }
    }
}
#endif
