using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    
    
    
    
}