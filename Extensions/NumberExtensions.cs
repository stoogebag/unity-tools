using UnityEngine;

namespace stoogebag.Extensions
{
    public static class NumberExtensions
    {
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static bool IsDivisible(this int i, int by)
        {
            return i % by == 0;
        }

        public static bool IsEven(this int i) => IsDivisible(i, 2);

        public static bool IsInt(this float f, out int rounded, float epsilon = 0.001f)
        {
            rounded = (int)Mathf.Round(f);
            return Mathf.Abs(f - rounded) <= epsilon;
        }
    }
}