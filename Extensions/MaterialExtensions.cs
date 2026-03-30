using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MaterialExtensions
{
    public static void SetVector3Texture(this Material mat, string propertyId, IEnumerable<Vector3> values, ref Texture2D tex)
    {
        var list = values as IList<Vector3> ?? values.ToList();
        int n = list.Count;

        if (n == 0)
        {
            mat.SetTexture(propertyId, null);
            return;
        }

        if (tex == null || tex.width != n || tex.height != 1 || tex.format != TextureFormat.RGBAFloat)
        {
            tex = new Texture2D(n, 1, TextureFormat.RGBAFloat, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
        }

        var data = new Color[n];
        for (int i = 0; i < n; i++)
        {
            var v = list[i];
            data[i] = new Color(v.x, v.y, v.z, 1f);
        }

        tex.SetPixelData(data, 0);
        tex.Apply(false, false);
        mat.SetTexture(propertyId, tex);
    }
}