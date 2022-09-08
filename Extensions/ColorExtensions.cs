using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stoogebag
{
    public static class ColorExtensions
    {
        /// <summary>
        /// gets the nearest colour by rgba componentwise least squares lol
        /// </summary>
        /// <param name="color"></param>
        /// <param name="???"></param>
        /// <returns></returns>
        public static Color GetNearest(this Color color, IEnumerable<Color> colors)
        {
            return colors.MinItem(c => c.Distance(color));
        }

        public static float Distance(this Color color, Color other)
        {
            return color.ToVector4().DistanceToSquared(other.ToVector4());
        }

        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }
        
    }
}