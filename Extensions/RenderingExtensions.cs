using System.Linq;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class RenderingExtensions
    {
        public static Material AddMaterial(this Renderer renderer, Material mat)
        {
            var mats = renderer.materials.AsEnumerable().Append(mat).ToArray();
            renderer.materials = mats;
            return renderer.materials[mats.Length - 1];
        }
        public static void RemoveMaterial(this Renderer renderer, Material mat)
        {
            var mats = renderer.materials.AsEnumerable().Where(m=>m!=mat).ToArray();
            renderer.materials = mats;
        }
        
        
    }
}