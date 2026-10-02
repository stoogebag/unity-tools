using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace stoogebag.GameState
{
    public static class SaveManager
    {
        public static IEnumerable<ISaveable> FindSaveables()
        {
            foreach (var behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (behaviour is ISaveable saveable)
                {
                    yield return saveable;
                }
            }
        }

        public static string Key(ISaveable saveable)
        {
            var component = (Component)saveable;
            return saveable.GetType().FullName + ":" + Path(component.transform);
        }

        public static Dictionary<string, object> CaptureAll()
        {
            var snapshot = new Dictionary<string, object>();

            foreach (var saveable in FindSaveables())
            {
                snapshot[Key(saveable)] = saveable.CaptureState();
            }

            return snapshot;
        }

        public static void RestoreAll(Dictionary<string, object> snapshot)
        {
            foreach (var saveable in FindSaveables())
            {
                if (snapshot.TryGetValue(Key(saveable), out var state))
                {
                    saveable.RestoreState(state);
                }
            }
        }

        public static string Path(Transform transform)
        {
            var builder = new StringBuilder();

            while (transform != null)
            {
                builder.Insert(0, "/" + transform.name);
                transform = transform.parent;
            }

            return builder.ToString();
        }
    }
}