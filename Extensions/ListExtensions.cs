using System;
using System.Collections.Generic;

namespace stoogebag.Extensions
{
    public static class ListExtensions
    {
        //
        // public static void DestroyGameObjectsAndClear<T>(this List<T> list) where T : Component
        // {
        //     for (var i = list.Count - 1; i >= 0; i--)
        //     {
        //         Destroy(list[i].gameObject);
        //     }
        //     
        //     list.DestroyAndClear();
        // }
    
        
        public static T GetFromRelativeIndex<T>(this IList<T> list, T current, Func<int,int> indexFunc)
        {
            var index = list.IndexOf(current);
            index = indexFunc(index).Mod(list.Count);
            return list[index];
        }

        public static T NextItem<T>(this IList<T> list, T current)
        {
            return GetFromRelativeIndex(list, current, t => t + 1);
        }
        public static T PrevItem<T>(this IList<T> list, T current)
        {
            return GetFromRelativeIndex(list, current, t => t - 1);
        }
        
        
        public static int Mod(this int a, int b)
        {
            return (a % b + b) % b;
        }
        
    }
}