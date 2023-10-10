using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace stoogebag.Extensions
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
        public static void ForAllChildrenRecursive<T>(this GameObject go, Action<T> action) {
            if (go == null) return;
            foreach (var trans in go.GetComponentsInChildren<T>(true)) {
                action(trans);
            }
        }

        public static void SetLayer(this GameObject go, int layer, bool includeChildren = false)
        {
            if (includeChildren) go.ForAllChildrenRecursive(t => t.layer = layer);
            else go.layer = layer;
        }

        public static int GetLayerMask(this GameObject go)
        {
            int layerMask = 1 << go.layer;
            return layerMask;
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
                if(name != null && t.name != name) return false;
                if (t.gameObject.TryGetComponent<T>(out var x)) return true;
                return false;
            })?.GetComponent<T>();
        }
        public static GameObject FirstOrDefault(this GameObject go, string name = null) // 
        {
            return go.transform.FirstOrDefault(name).gameObject;
        }

        public static T GetChild<T>(this MonoBehaviour component, string name = null) where T: Component
        {
            return component.gameObject.FirstOrDefault<T>(name);
        }
        public static GameObject GetChild(this MonoBehaviour component, string name = null) 
        {
            return component.gameObject.FirstOrDefault(name);
        }
        public static GameObject GetChildWithComponent<T>(this MonoBehaviour component, string name = null) where T: Component
        {
            return component.GetChild<T>(name).gameObject;
        }

        public static IEnumerable<GameObject> GetChildrenWithComponent<T>(this MonoBehaviour component, bool includeInactive = false)
            where T : Component
        {
            return component.gameObject.GetComponentsInChildren<T>(includeInactive).Select(t => t.gameObject);
        }

        public static T FirstOrDefault<T>(this GameObject go, Func<string,bool> nameCondition) // 
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

        public static IEnumerable<Transform> GetAllDescendents(this Transform transform, Func<Transform, bool> query = null)
        {
            if (query == null || query(transform)) {
                yield return transform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = GetAllDescendents(transform.GetChild(i), query);
                foreach (var t in result)
                {
                    yield return t;
                }
            }
        }

        

        public static Vector3 PositionOffset(this GameObject me, GameObject other)
        {
            return other.transform.position - me.transform.position;
        }
        public static float DistanceTo(this GameObject me, GameObject other)
        {
            return me.PositionOffset(other).magnitude;
        }

        public static void SetLocalScaleX(this Transform me, float val)
        {
            me.localScale = new Vector3(val, me.localScale.y, me.localScale.z);
        }
        
        
        public static void SetLocalScaleY(this Transform me, float val)
        {
            me.localScale = new Vector3(me.localScale.x, val, me.localScale.z);
        }
        public static void SetLocalScaleZ(this Transform me, float val)
        {
            me.localScale = new Vector3(me.localScale.x, me.localScale.y, val);
        }

        //
        // public static List<T> FindAllPrefabs<T>() where T: MonoBehaviour
        // {
        //     var all = Resources.FindObjectsOfTypeAll<T>();
        //     all.Where(go=>
        //     EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)))
        // }
        //

        public static IDisposable TemporarilyScaleDisposable(this Transform t, Vector3 scale)
        {
            t.localScale = t.localScale.ScaleByVector(scale);
            return Disposable.Create(() =>
            {
                t.localScale = t.localScale.ScaleByVector(scale.Invert());
            });
        }


        public static T GetOrAddComponent<T>(this GameObject go)  where T: Component
        {
            if (go.TryGetComponent<T>(out var component)) return component;
            else
            {
                var t = go.AddComponent<T>();
                return t;
            }
        }

        //weird find function for sugar/convenience. use like "MyProperty = MyProperty.Find(name)"
        public static T FindByName<T>(this T obj, string name, bool includeInactive = true) where T : Component
        {
            if (!includeInactive)
            {
                var g = GameObject.Find(name).GetComponent<T>();
                return g;
            }
            else return FindIncludingInactive<T>(obj, name);
        }
        
        
        //this one actually works on inactive. WARNING!! may accidentally find prefabs i think.
        //todo: if theres an issue might look at checking obj.gameObject.scene
        private static T FindIncludingInactive<T>(T obj, string name) where T:Component
        {
            var objs = Resources.FindObjectsOfTypeAll(typeof(T)).Cast<T>();
 
            foreach (var t in objs)
            {
                if (t.name == name)
                {
                    return t;
                }
            }
 
            return null;
        }

        public static void TrimChildNames(this GameObject go)
        {
            go.ForAllChildrenRecursive(t=>t.name = t.name.Trim());
        }


        public static T TryGetOrAddComponent<T>(this GameObject go) where T:Component
        {
            if (go.TryGetComponent<T>(out var t)) return t;
            else return go.AddComponent<T>();
        }
        
    }
    
}
