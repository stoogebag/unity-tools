using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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
    
}