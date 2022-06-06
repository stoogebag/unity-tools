using System;
using System.Collections.Generic;
using UnityEngine;

namespace stoogebag
{
    public static class GameObjectExtensions { 
        //public static string TryGetGuid(this GameObject me)
        //{
        //    var gc = me.GetComponent<GuidComponent>();
        //}
        public static void DestroyAllAndClear(this List<GameObject> me)
        {
            foreach (var go in me)
            {
                UnityEngine.GameObject.Destroy(go);
            }
            me.Clear();
        }
        
        public static void ForAllChildrenRecursive(this GameObject go, Action<GameObject> action) {
            if (go == null) return;
            foreach (var trans in go.GetComponentsInChildren<Transform>(true)) {
                action(trans.gameObject);
            }
        }

        public static void SetLayer(this GameObject go, int layer, bool includeChildren = false)
        {
            if (includeChildren) go.ForAllChildrenRecursive(t => t.layer = layer);
            else go.layer = layer;
        }
    
        public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
        {
            if (query(transform)) {
                return transform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = FirstOrDefault(transform.GetChild(i), query);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
