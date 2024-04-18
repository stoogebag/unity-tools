using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class DebugExtensions
    {
        
        public static void DrawRay(Ray ray, float distance, Color color = default)
        {
            if(color == default) color = Color.red;
            Debug.DrawLine(ray.origin, ray.origin + ray.direction.normalized* distance, color);
        }
        
        public static void LogList<T>(this IEnumerable<T> list, Func<T,string> toString = null)
        {
            Debug.Log($"[{string.Join(", ", list.Select(t=>toString?.Invoke(t) ?? t.ToString()))}]");
        }
        
    }
}