using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class BoundsExtensions
    {
        public static Bounds GetBounds(IEnumerable<Vector3> points)
        {
            if (!points.Any())
            {
                return new Bounds();
            }

            var min = points.First();
            var max = points.First();

            foreach (var point in points)
            {
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            return new Bounds((min + max) / 2, max - min);
        }
    
        public static Bounds Combine(IEnumerable<Bounds> bounds)
        {
            if (!bounds.Any()) return new Bounds(); //should this be?! or should i throw.

            Bounds overallBounds = bounds.First();

            foreach (var bound in bounds)
            {
                overallBounds.Encapsulate(bound);
            }
            
            return overallBounds;
        }
    }
}