using System;
using System.Collections.Generic;
using System.Linq;

namespace stoogebag
{

    public static class DictionaryExtensions
    {
        public static TValue TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, Func<TValue> addNew)
        {
            if (dic.TryGetValue(key, out var val)) return val;
            else
            {
                var newVal = addNew();
                dic[key] = newVal;
                return newVal;
            }
        }

        public static TValue TryGetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.TryGetValue(key, out var val)) return val;
            else return default;
        }

    }

    public static class EnumerableExtensions
    {
        //public static HashSet<T> ToHashSet<T>(this IEnumerable<T> me)
        //{
        //    var hs = new HashSet<T>();
        //    hs.AddRange(me);
        //    return hs;
        //}

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

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> me)
        {
            return me.Where(t => t != null);
        }

    }

}
