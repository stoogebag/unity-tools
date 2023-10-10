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
        
        
    }
}