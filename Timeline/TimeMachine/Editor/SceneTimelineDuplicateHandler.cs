using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using System.Reflection;

namespace StoogeBag.Timeline.Editor
{
    /// <summary>
    /// Ensures that duplicating a GameObject carrying a <see cref="SceneTimeline"/> produces an
    /// independent, deep-copied <see cref="TimelineAsset"/> instead of sharing the original reference.
    ///
    /// Unity's default Ctrl+D only copies the serialized field value, and since <c>timelineAsset</c>
    /// is a ScriptableObject reference, the duplicate ends up pointing at the SAME TimelineAsset
    /// instance. <c>Object.Instantiate(timelineAsset)</c> is insufficient because the tracks (separate
    /// ScriptableObjects) keep referencing the originals, so we use the Timeline package's own deep
    /// clone: <c>TrackAsset.Duplicate(...)</c> (TrackExtensions.cs), which recursively clones tracks,
    /// subtracks, clips, clip assets, curves and markers and registers them as sub-assets of the new
    /// timeline. Track bindings on the PlayableDirector are remapped to the cloned tracks.
    /// </summary>
    [InitializeOnLoad]
    public static class SceneTimelineDuplicateHandler
    {
        static SceneTimelineDuplicateHandler()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static bool _processing;

        // TrackAsset.Duplicate is internal to the Unity.Timeline.Editor assembly.
        private static MethodInfo s_duplicateMethod;
        private static MethodInfo DuplicateMethod
        {
            get
            {
                if (s_duplicateMethod == null)
                {
                    var asm = Assembly.Load("Unity.Timeline.Editor");
                    var t = asm?.GetType("UnityEditor.Timeline.TrackExtensions");
                    s_duplicateMethod = t?.GetMethod(
                        "Duplicate",
                        BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        new[] { typeof(TrackAsset), typeof(IExposedPropertyTable), typeof(IExposedPropertyTable), typeof(TimelineAsset) },
                        null);
                }
                return s_duplicateMethod;
            }
        }

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

        // ---- Deep clone (driven by the engine's own TrackAsset.Duplicate) -----------

        /// <summary>
        /// Creates a fully independent copy of <paramref name="src"/>. Track bindings on
        /// <paramref name="dstDirector"/> are remapped from the source tracks to the cloned tracks
        /// (preserving the bound object Unity already remapped on duplicate).
        /// </summary>
        public static TimelineAsset DeepCloneTimeline(TimelineAsset src, PlayableDirector srcDirector, PlayableDirector dstDirector)
        {
            var dst = ScriptableObject.CreateInstance<TimelineAsset>();
            dst.name = src.name + " (Clone)";

            var trackMap = new Dictionary<TrackAsset, TrackAsset>();

            foreach (var rootTrack in src.GetRootTracks())
            {
                var newTrack = InvokeDuplicate(rootTrack, srcDirector, dstDirector, dst);
                if (newTrack != null)
                    MapTracks(rootTrack, newTrack, trackMap);
            }

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

        private static TrackAsset InvokeDuplicate(TrackAsset srcTrack, PlayableDirector srcDirector, PlayableDirector dstDirector, TimelineAsset dstAsset)
        {
            var method = DuplicateMethod;
            if (method == null)
            {
                Debug.LogError("[SceneTimelineDuplicateHandler] Could not find UnityEditor.Timeline.TrackExtensions.Duplicate.");
                return null;
            }

            return (TrackAsset)method.Invoke(null, new object[]
            {
                srcTrack,
                srcDirector,
                dstDirector,
                dstAsset
            });
        }

        private static void MapTracks(TrackAsset src, TrackAsset dst, Dictionary<TrackAsset, TrackAsset> map)
        {
            map[src] = dst;

            var srcChildren = new List<TrackAsset>(src.GetChildTracks());
            var dstChildren = new List<TrackAsset>(dst.GetChildTracks());
            for (int i = 0; i < srcChildren.Count && i < dstChildren.Count; i++)
                MapTracks(srcChildren[i], dstChildren[i], map);
        }
    }
}
