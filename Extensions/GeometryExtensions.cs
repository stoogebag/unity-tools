using UnityEngine;

namespace stoogebag.Extensions
{
    public static class GeometryExtensions
    {
        
        
        public static Bounds GetBounds(this GameObject go, bool local = false)
        {
            Bounds bounds = default;

            foreach (var r in go.GetComponentsInChildren<MeshRenderer>())
            {
                var b = local ? r.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds : r.bounds;

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(b);
            }

            foreach (var r in go.GetComponentsInChildren<Terrain>())
            {
                var b = local ? new Bounds(r.terrainData.size / 2, r.terrainData.size) : new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size);

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size));

            }

            if (bounds == default)
                bounds.center = go.transform.position;

            return bounds;
        }
        
    }
}