using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UnityEditor;

public class CreateTextureFromColors
{

    public static Color[] Colors = new[] { Color.red, Color.yellow, Color.magenta, Color.blue, Color.green, };
    
    public static Texture2D CreateTexFromColorArray(Color[] colors)
    {
        var texture = new Texture2D(colors.Length, 1, TextureFormat.ARGB32, false);
 
        for (var i = 0; i < colors.Length; i++)
        {
            texture.SetPixel(i, 0, colors[i]);
        }
        
        // Apply all SetPixel calls
        texture.Apply();

        return texture;
    }

    public static void SaveColorsToTex(Color[] colors, string path = "Assets/colors.asset")
    {
        var tex = CreateTexFromColorArray(colors);
        AssetDatabase.CreateAsset(tex, path);
    }

    public static void SaveVectorsToTex(IEnumerable<Vector4> vectors, int width, int height, string path)
    {
        var index = 0;
        
        var texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        foreach (var vector4 in vectors)
        {
            var x = index % width;
            var y = index / width;
            index++;
            texture.SetPixel(x,y,vector4);
        }
        texture.Apply();
        AssetDatabase.CreateAsset(texture, path);

    }
    
}
