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

        public static Transform FirstOrDefault(this Transform transform, string name)
        {
            return transform.FirstOrDefault(t => t.name == name);
        }

        public static T FirstOrDefault<T>(this GameObject go, string name = null) where T:class// 
        {
            if (typeof(T) == typeof(GameObject)) return go.FirstOrDefault(name) as T;
            return go.transform.FirstOrDefault(t =>
            {
                if(t.name != name) return false;
                if (t.gameObject.TryGetComponent<T>(out var x)) return true;
                return false;
            }).GetComponent<T>();
        }
        public static GameObject FirstOrDefault(this GameObject go, string name = null) // 
        {
            return go.transform.FirstOrDefault(name).gameObject;
        }

        public static T FirstOrDefault<T>(this GameObject go, Func<string,bool> nameCondition = null) // 
        {
            return go.transform.FirstOrDefault(t =>
            {
                if(!nameCondition(t.name)) return false;
                if (t.gameObject.TryGetComponent<T>(out var x)) return true;
                return false;
            }).GetComponent<T>();
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
