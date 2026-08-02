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

        private GameObject _gridPlane;

        [SerializeField] private GameObject _spawnParent;
        private float _gridY;
        private float _placementRotation;
        private bool _active;

        [MenuItem("stooge/Grid Level Editor")]
        public static void ShowWindow()
        {
            var w = GetWindow<GridLevelEditorWindow>();
            w.titleContent = new GUIContent("Grid Level Editor");
            w.minSize = new Vector2(280, 400);
        }

        private void OnEnable()
        {
            SceneView.beforeSceneGui += OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewRepaint;
            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            SceneView.beforeSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui -= OnSceneViewRepaint;
            Undo.undoRedoPerformed -= Repaint;
            DestroyGhost();
            DestroyGridPlane();
        }

        private void OnDestroy()
        {
            SceneView.beforeSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui -= OnSceneViewRepaint;
            Undo.undoRedoPerformed -= Repaint;
            DestroyGhost();
            DestroyGridPlane();
        }

        private void OnGUI()
        {
            var evt = Event.current;
            if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.BackQuote)
            {
                ToggleActive();
                evt.Use();
                Repaint();
            }

            if (evt.type == EventType.KeyDown)
                HandleNumberKey(evt);

            DrawToolbar();
            DrawSpawnParentField();
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

        private void DrawSpawnParentField()
        {
            _spawnParent = (GameObject)EditorGUILayout.ObjectField("Spawn Parent", _spawnParent, typeof(GameObject), true);
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
                if (changed) { DestroyGhost(); _placementRotation = 0f; }
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

            if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.BackQuote)
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
                    if (evt.keyCode == KeyCode.PageUp)   { _gridY += GridSpacing; evt.Use(); Repaint(); }
                    if (evt.keyCode == KeyCode.PageDown)  { _gridY -= GridSpacing; evt.Use(); Repaint(); }
                    if (evt.keyCode == KeyCode.Q) { _placementRotation -= 90f; evt.Use(); Repaint(); }
                    if (evt.keyCode == KeyCode.E) { _placementRotation += 90f; evt.Use(); Repaint(); }
                    HandleNumberKey(evt);
                }

                if (evt.type == EventType.MouseDown && evt.button == 0 && evt.shift && !evt.alt)
                {
                    var hit = HandleUtility.PickGameObject(evt.mousePosition, false);
                    if (hit != null)
                    {
                        var root = PrefabUtility.GetNearestPrefabInstanceRoot(hit);
                        if (root != null && root.hideFlags == HideFlags.HideAndDontSave)
                        {
                            var renderers = root.GetComponentsInChildren<Renderer>();
                            foreach (var r in renderers) r.enabled = false;
                            hit = HandleUtility.PickGameObject(evt.mousePosition, false);
                            root = hit != null ? PrefabUtility.GetNearestPrefabInstanceRoot(hit) : null;
                            foreach (var r in renderers) r.enabled = true;
                        }

                        var source = root != null ? PrefabUtility.GetCorrespondingObjectFromSource(root) : null;

                        var check = root?.transform.parent;
                        while ((source == null || !_prefabSlots.Contains(source)) && check != null)
                        {
                            var checkRoot = PrefabUtility.GetNearestPrefabInstanceRoot(check.gameObject);
                            source = checkRoot != null ? PrefabUtility.GetCorrespondingObjectFromSource(checkRoot) : null;
                            if (source != null && _prefabSlots.Contains(source))
                                root = checkRoot;
                            check = check.parent;
                        }

                        Debug.Log($"deletion: hit={hit?.name} root={root?.name} source={source?.name} inSlots={source != null && _prefabSlots.Contains(source)}");
                        if (source != null && _prefabSlots.Contains(source))
                        {
                            Undo.DestroyObjectImmediate(root);
                            evt.Use();
                        }
                    }
                }

                if (_selectedSlot >= 0 && _selectedSlot < _prefabSlots.Count)
                {
                    var prefab = _prefabSlots[_selectedSlot];
                    if (prefab != null)
                    {
                        if (evt.shift)
                            DestroyGhost();
                        else
                            HandlePlacement(sceneView, prefab, evt, controlId);
                    }
                }
            }
        }

        private void OnSceneViewRepaint(SceneView sceneView)
        {
            if (!_active)
            {
                DestroyGridPlane();
                return;
            }

            UpdateGridPlane(sceneView);
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

        private void UpdateGridPlane(SceneView sceneView)
        {
            if (_gridPlane == null)
            {
                CreateGridPlane();
                if (_gridPlane == null) return;
            }

            var camPos = sceneView.camera.transform.position;
            var cx = Mathf.Round((camPos.x - 5f) / GridSpacing) * GridSpacing + 5f;
            var cz = Mathf.Round((camPos.z - 5f) / GridSpacing) * GridSpacing + 5f;
            _gridPlane.transform.position = new Vector3(cx, _gridY, cz);
        }

        private void CreateGridPlane()
        {
            const float halfSize = 200f;
            const float lineWidth = 0.12f;
            const float denseSpacing = 2.5f;

            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                var i = verts.Count;
                verts.Add(a); verts.Add(b); verts.Add(c); verts.Add(d);
                uvs.Add(Vector2.zero); uvs.Add(Vector2.zero); uvs.Add(Vector2.zero); uvs.Add(Vector2.zero);
                tris.AddRange(new[] { i, i + 2, i + 1, i, i + 3, i + 2 });
            }

            // Fill quad
            AddQuad(
                new Vector3(-halfSize, 0, -halfSize),
                new Vector3( halfSize, 0, -halfSize),
                new Vector3( halfSize, 0,  halfSize),
                new Vector3(-halfSize, 0,  halfSize)
            );

            var halfW = lineWidth * 0.5f;

            void AddLineQuad(float pos, bool alongZ)
            {
                if (alongZ)
                    AddQuad(
                        new Vector3(pos - halfW, 0, -halfSize),
                        new Vector3(pos + halfW, 0, -halfSize),
                        new Vector3(pos + halfW, 0,  halfSize),
                        new Vector3(pos - halfW, 0,  halfSize)
                    );
                else
                    AddQuad(
                        new Vector3(-halfSize, 0, pos - halfW),
                        new Vector3( halfSize, 0, pos - halfW),
                        new Vector3( halfSize, 0, pos + halfW),
                        new Vector3(-halfSize, 0, pos + halfW)
                    );
            }

            for (var x = -halfSize; x <= halfSize; x += denseSpacing) AddLineQuad(x, true);
            for (var z = -halfSize; z <= halfSize; z += denseSpacing) AddLineQuad(z, false);
            for (var x = -halfSize; x <= halfSize; x += GridSpacing) AddLineQuad(x, true);
            for (var z = -halfSize; z <= halfSize; z += GridSpacing) AddLineQuad(z, false);

            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.hideFlags = HideFlags.HideAndDontSave;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(1, 0.15f, 0.15f, 0.2f);
            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_ZWrite", 0);
            mat.SetInt("_AlphaClip", 0);
            mat.SetFloat("_Cull", 0);
            mat.renderQueue = 3000;
            mat.hideFlags = HideFlags.HideAndDontSave;

            _gridPlane = new GameObject("GRID_PLANE");
            _gridPlane.hideFlags = HideFlags.HideAndDontSave;
            _gridPlane.AddComponent<MeshFilter>().sharedMesh = mesh;
            _gridPlane.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private void DestroyGridPlane()
        {
            if (_gridPlane != null)
            {
                var filter = _gridPlane.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null)
                    DestroyImmediate(filter.sharedMesh);
                DestroyImmediate(_gridPlane);
                _gridPlane = null;
            }
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
            _placementGhost.transform.rotation = Quaternion.Euler(0, _placementRotation, 0);
            _showGhost = true;
        }

        private static Material GetGhostMaterial()
        {
            if (_ghostMaterial == null)
            {
                _ghostMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                _ghostMaterial.color = new Color(0.3f, 0.7f, 1f, 0.15f);
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
            go.transform.rotation = Quaternion.Euler(0, _placementRotation, 0);
            if (_spawnParent != null)
            {
                var typeFolder = _spawnParent.transform.Find($"-{prefab.name}");
                if (typeFolder == null)
                {
                    typeFolder = new GameObject($"-{prefab.name}").transform;
                    typeFolder.SetParent(_spawnParent.transform);
                    Undo.RegisterCreatedObjectUndo(typeFolder.gameObject, $"Create {prefab.name} folder");
                }
                go.transform.SetParent(typeFolder, true);
            }
            Undo.RegisterCreatedObjectUndo(go, $"Place {prefab.name}");
            Selection.activeGameObject = go;
        }

        private void HandleNumberKey(Event evt)
        {
            var key = evt.keyCode;
            var index = key - KeyCode.Alpha1;
            if (index < 0 || index > 8 || index >= _prefabSlots.Count) return;

            if (_selectedSlot != index) { DestroyGhost(); _placementRotation = 0f; }
            _selectedSlot = index;
            evt.Use();
            Repaint();
            SceneView.RepaintAll();
        }

        private void ToggleActive()
        {
            _active = !_active;
            if (!_active) { DestroyGhost(); DestroyGridPlane(); }
            else Selection.activeGameObject = null;
            ShowNotification(new GUIContent(_active ? "Grid Editor: Active" : "Grid Editor: Off"), 1f);
            SceneView.lastActiveSceneView?.ShowNotification(new GUIContent(_active ? "Grid Editor: Active" : "Grid Editor: Off"), 1f);
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
