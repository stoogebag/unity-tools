using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StoogeBag.Timeline.Editor
{
    /// <summary>
    /// Ensures that duplicating a GameObject carrying a <see cref="SceneTimeline"/> produces an
    /// independent, deep-copied <see cref="TimelineAsset"/> instead of sharing the original reference.
    ///
    /// Unity's default Ctrl+D only copies the serialized field value, and since <c>timelineAsset</c>
    /// is a ScriptableObject reference, the duplicate ends up pointing at the SAME TimelineAsset
    /// instance. <c>Object.Instantiate(timelineAsset)</c> is insufficient because tracks/clips keep
    /// referencing the original TrackAsset/clip instances, so we recursively rebuild the timeline.
    /// </summary>
    [InitializeOnLoad]
    public static class SceneTimelineDuplicateHandler
    {
        static SceneTimelineDuplicateHandler()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static bool _processing;

        // TrackAsset.CreateClip(Type) and AddMarker are internal to the Timeline assembly.
        private static readonly System.Reflection.MethodInfo s_createClip =
            typeof(TrackAsset).GetMethod("CreateClip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(System.Type) }, null);
        private static readonly System.Reflection.MethodInfo s_addMarker =
            typeof(TrackAsset).GetMethod("AddMarker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // ---- Automatic detection -------------------------------------------------

        private static void OnHierarchyChanged()
        {
            if (_processing || Application.isPlaying)
                return;

            _processing = true;
            try
            {
                ProcessDuplicates();
            }
            finally
            {
                _processing = false;
            }
        }

        private static void ProcessDuplicates()
        {

            var all = Object.FindObjectsByType<SceneTimeline>(FindObjectsSortMode.None);

            // Group SceneTimelines by the TimelineAsset instance they reference.
            var groups = new Dictionary<int, List<SceneTimeline>>();
            foreach (var st in all)
            {
                // Don't touch timelines being edited inside a prefab asset.
                if (PrefabUtility.IsPartOfPrefabAsset(st.gameObject))
                    continue;
                if (st.TimelineAsset == null)
                    continue;
                int id = st.TimelineAsset.GetInstanceID();
                if (!groups.TryGetValue(id, out var list))
                    groups[id] = list = new List<SceneTimeline>();
                list.Add(st);
            }

            foreach (var group in groups.Values)
            {
                if (group.Count <= 1)
                    continue;

                // Keep the first owner intact; deep-copy for every other sharer.
                for (int i = 1; i < group.Count; i++)
                {
                    var sharer = group[i];
                    var director = sharer.Director;
                    var clone = DeepCloneTimeline(sharer.TimelineAsset, director, director);

                    Undo.RegisterCreatedObjectUndo(clone, "Duplicate SceneTimeline");
                    Undo.RecordObject(sharer, "Duplicate SceneTimeline");
                    sharer.AssignTimelineAsset(clone);
                    EditorUtility.SetDirty(sharer);
                }
            }
        }

        // ---- Explicit button -----------------------------------------------------

        [UnityEditor.MenuItem("CONTEXT/SceneTimeline/Duplicate Timeline")]
        private static void DuplicateTimelineMenu(MenuCommand command)
        {
            var st = command.context as SceneTimeline;
            if (st == null || st.TimelineAsset == null)
                return;

            var director = st.Director;
            var clone = DeepCloneTimeline(st.TimelineAsset, director, director);

            Undo.RegisterCreatedObjectUndo(clone, "Duplicate Timeline");
            Undo.RecordObject(st, "Duplicate Timeline");
            st.AssignTimelineAsset(clone);
            EditorUtility.SetDirty(st);
        }

        // ---- Deep clone -----------------------------------------------------------

        /// <summary>
        /// Creates a fully independent copy of <paramref name="src"/>, cloning tracks, clips, clip
        /// assets and markers. Track bindings on <paramref name="dstDirector"/> are remapped from the
        /// source tracks to the cloned tracks (preserving the bound object Unity already remapped on
        /// duplicate).
        /// </summary>
        public static TimelineAsset DeepCloneTimeline(TimelineAsset src, PlayableDirector srcDirector, PlayableDirector dstDirector)
        {
            var dst = ScriptableObject.CreateInstance<TimelineAsset>();
            dst.name = src.name + " (Clone)";

            var trackMap = new Dictionary<TrackAsset, TrackAsset>();

            foreach (var rootTrack in src.GetRootTracks())
                CloneTrack(rootTrack, null, dst, trackMap);

            // Remap track bindings to the cloned tracks (and drop the stale original-track bindings).
            if (dstDirector != null)
            {
                foreach (var kvp in trackMap)
                {
                    var binding = dstDirector.GetGenericBinding(kvp.Key);
                    if (binding == null && srcDirector != null)
                        binding = srcDirector.GetGenericBinding(kvp.Key);

                    dstDirector.ClearGenericBinding(kvp.Key);
                    if (binding != null)
                        dstDirector.SetGenericBinding(kvp.Value, binding);
                }
            }

            return dst;
        }

        private static TrackAsset CloneTrack(TrackAsset srcTrack, TrackAsset dstParent, TimelineAsset dstAsset, Dictionary<TrackAsset, TrackAsset> trackMap)
        {
            var dstTrack = dstAsset.CreateTrack(srcTrack.GetType(), dstParent, srcTrack.name);

            // Copy user-serialized fields, but skip the structural fields that CreateTrack already
            // set up correctly (m_Parent, m_Children, m_Clips) and m_Markers (cloned separately below)
            // so we don't re-link to the originals.
            using (var soSrc = new SerializedObject(srcTrack))
            using (var soDst = new SerializedObject(dstTrack))
            {
                var prop = soSrc.GetIterator();
                while (prop.NextVisible(true))
                {
                    if (prop.name == "m_Parent" || prop.name == "m_Children" || prop.name == "m_Clips" || prop.name == "m_Markers")
                        continue;
                    soDst.CopyFromSerializedProperty(prop);
                }
                soDst.ApplyModifiedProperties();
            }

            // Deep-copy the track's curves clip (shared by reference otherwise).
            using (var soSrc = new SerializedObject(srcTrack))
            {
                var srcCurves = soSrc.FindProperty("m_Curves")?.objectReferenceValue as AnimationClip;
                if (srcCurves != null)
                {
                    var newCurves = Object.Instantiate(srcCurves);
                    newCurves.name = srcCurves.name;
                    using (var soDst = new SerializedObject(dstTrack))
                    {
                        soDst.FindProperty("m_Curves").objectReferenceValue = newCurves;
                        soDst.ApplyModifiedProperties();
                    }
                }
            }

            trackMap[srcTrack] = dstTrack;

            // Clone clips. TimelineClip is not a UnityEngine.Object, so we create each clip on the
            // destination track via CreateClip (which wires the parent correctly) and then copy its
            // serialized fields through the track's m_Clips array, deep-copying the clip asset and
            // override curves.
            using (var soSrc = new SerializedObject(srcTrack))
            using (var soDst = new SerializedObject(dstTrack))
            {
                var srcClips = soSrc.FindProperty("m_Clips");
                var dstClips = soDst.FindProperty("m_Clips");

                for (int i = 0; i < srcClips.arraySize; i++)
                {
                    var srcElem = srcClips.GetArrayElementAtIndex(i);
                    var srcAsset = srcElem.FindPropertyRelative("m_Asset").objectReferenceValue;
                    if (srcAsset == null)
                        continue;

                    // TrackAsset.CreateClip(Type) is internal, so invoke it via reflection. It appends
                    // a properly-wired clip as the last element of m_Clips.
                    s_createClip.Invoke(dstTrack, new object[] { srcAsset.GetType() });
                    soDst.Update();

                    var dstElem = dstClips.GetArrayElementAtIndex(dstClips.arraySize - 1);

                    // Copy all serialized clip fields (m_Asset / m_AnimationCurves ride along by
                    // reference and are deep-copied below).
                    dstElem.CopyFromSerializedProperty(srcElem);

                    // The clip must belong to the new track, not the source.
                    dstElem.FindPropertyRelative("m_ParentTrack").objectReferenceValue = dstTrack;

                    // Deep-copy the clip's playable asset.
                    var newAsset = Object.Instantiate(srcAsset);
                    newAsset.name = srcAsset.name;
                    EditorUtility.CopySerialized(srcAsset, newAsset);
                    dstElem.FindPropertyRelative("m_Asset").objectReferenceValue = newAsset;

                    // Deep-copy the clip's override curves.
                    var srcCurves = srcElem.FindPropertyRelative("m_AnimationCurves").objectReferenceValue as AnimationClip;
                    if (srcCurves != null)
                    {
                        var newCurves = Object.Instantiate(srcCurves);
                        newCurves.name = srcCurves.name;
                        dstElem.FindPropertyRelative("m_AnimationCurves").objectReferenceValue = newCurves;
                    }

                    soDst.ApplyModifiedProperties();
                }
            }

            // Clone markers (deep copy so edits don't bleed into the source).
            if (s_addMarker != null)
            {
                foreach (var marker in srcTrack.GetMarkers())
                {
                    var newMarker = Object.Instantiate(marker as ScriptableObject);
                    s_addMarker.Invoke(dstTrack, new object[] { newMarker });
                }
            }

            // Recurse into subtracks.
            foreach (var child in srcTrack.GetChildTracks())
                CloneTrack(child, dstTrack, dstAsset, trackMap);

            return dstTrack;
        }
    }
}
