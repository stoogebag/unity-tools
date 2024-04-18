using System.Collections.Generic;
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
        
        public static void LogList<T>(this IEnumerable<T> list)
        {
            Debug.Log($"[{string.Join(", ", list)}]");
        }
        
    }
}