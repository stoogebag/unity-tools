using UnityEngine;

namespace stoogebag.Extensions
{
    public static class FloatExtensions
    {
        
        public static bool EqualsApproximately(this float f, float other, float epsilon = 0.0001f)
        {
            return Mathf.Abs(f - other) < epsilon;
        }
        
        public static bool ApproximatelyZero(this float f, float epsilon = 0.0001f)
        {
            return Mathf.Abs(f) < epsilon;
        }
    }
}