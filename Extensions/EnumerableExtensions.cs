using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace stoogebag.Extensions
{
    public static class EnumerableExtensions
    {
        public static T MinItem<T>(this IEnumerable<T> me, Func<T, float> valuation)
        {
            T minItem = me.First();
            var min = float.MaxValue;
            foreach (var t in me)
            {
                var val = valuation(t);
                if (val < min)
                {
                    min = val;
                    minItem = t;
                }
            }

            return minItem;

        }

        public static T MaxItem<T>(this IEnumerable<T> me, Func<T, float> valuation)
        {
            return MinItem(me, t => -valuation(t));
        }
        
        public static void DestroyAndClear<T>(this List<T> list) where T : Object
        {
            foreach (var o in list)
            {
                UnityEngine.Object.Destroy(o);
            }
            list.Clear();
        }
        public static void DestroyAndClearGameObjects<T>(this List<T> list) where T : Component
        {
            foreach (var o in list)
            {
                UnityEngine.Object.Destroy(o.gameObject);
            }
            list.Clear();
        }
        public static void DestroyImmediateAndClearGameObjects<T>(this List<T> list) where T : Component
        {
            foreach (var o in list)
            {
                if (o == null) continue;
                UnityEngine.Object.DestroyImmediate(o.gameObject);
            }
            list.Clear();
        }

        public static void DisposeWith(this IDisposable d, Component component)
        {
            component.OnDestroyAsObservable().Subscribe(u => d.Dispose());
        }
    
        // Ensures that the capacity of this list is at least the given minimum 
        // value. If the currect capacity of the list is less than min, the
        // capacity is increased to twice the current capacity or to min, 
        // whichever is larger. 
        public static void EnsureCapacity<T>(this List<T> list, int min)
        {
            if (list.Capacity < min) list.Capacity = min;
        }

        
        public static void AddRange<T>(this HashSet<T> me, IEnumerable<T> e)
        {
            foreach (var t in e)
            {
                me.Add(t);
            }

        }

        public static int IndexOfFirst<T>(this IEnumerable<T> me, Func<T, bool> condition)
        {
            int i = 0;
            foreach(var x in me)
            {
                if (condition(x)) return i;
                i++;
            }
            return -1;

        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> me) where T: class
        {
            return me.Where(t => t != null);
        }

        public static IEnumerable<T> One<T>(this T t)
        {
            yield return t;
        }

        public static IEnumerable<T> Repeat<T>(this T t, int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return t;
            }
        }

        public static HashSet<T> ToHashSet<X,T>(this IEnumerable<X> me, Func<X,T> predicate)
        {
            return new HashSet<T>(me.Select(predicate));
        }

        public static IEnumerable<T> GetAllDescendants<T>(this IEnumerable<T> source, Func<T,IEnumerable<T>> childrenFunc)
        {
            if (source == null) yield break;
            foreach (var s in source)
            {
                yield return s;
                foreach (var t in GetAllDescendants(childrenFunc(s), childrenFunc))
                {
                    if (t == null) continue;
                    yield return t;
                }
            }
        }
        
        public static IEnumerable<T> GetAllDescendants<T>(this T source, Func<T,IEnumerable<T>> childrenFunc, bool excludeOriginal = true)
        {
            if (source == null) yield break;
            if (!excludeOriginal) yield return source;
            foreach (var s in childrenFunc(source))
            {
                yield return s;
                foreach (var t in GetAllDescendants(childrenFunc(s), childrenFunc))
                {
                    if (t == null) continue;
                    yield return t;
                }
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T source, params T[] others)
        {
            yield return source;
            for (var i = 0; i < others.Length; i++)
            {
                yield return others[i];
            }
        }


    }
}