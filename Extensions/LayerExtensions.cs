using UnityEngine;

namespace stoogebag
{
    public static class LayerExtensions
    {
        
        public static int AddLayerToLayerMask(this int layerMask, int layer)
        {
            return layerMask | 1 << layer;
        }
   
        public static int RemoveLayerFromLayerMask(this int layerMask, int layer)
        {
            return layerMask & ~(1 << layer);
        }
 
        public static void AddLayerToCullingMask(this Camera camera, int layer)
        {
            camera.cullingMask |= 1 << layer;
        }
   
        public static void RemoveLayerFromCullingMask(this Camera camera, int layer)
        {
            camera.cullingMask &= ~(1 << layer);
        }
        
        
    }
}