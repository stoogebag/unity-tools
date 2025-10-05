using UnityEngine;
using UnityEditor;

public class SpriteChromaKeyProcessor
{
    [MenuItem("Assets/Convert Top Left Pixel To Transparent Alpha", false, 1000)]
    public static void ConvertTopLeftPixelToTransparent()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

                if (importer == null || importer.textureType != TextureImporterType.Sprite)
                {
                    Debug.LogWarning($"Asset '{obj.name}' is not a sprite texture.");
                    continue;
                }

                bool wasReadable = importer.isReadable;
                importer.isReadable = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.SaveAndReimport();

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                
                // Use top-left pixel as chroma key (texture coordinates: y=0 is bottom)
                Color chromaKey = tex.GetPixel(0, tex.height - 1);
                float tolerance = 0.1f;

                Color[] pixels = tex.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (Vector3.Distance(new Vector3(pixels[i].r, pixels[i].g, pixels[i].b),
                        new Vector3(chromaKey.r, chromaKey.g, chromaKey.b)) < tolerance)
                    {
                        pixels[i] = Color.clear;
                    }
                }

                Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
                newTex.SetPixels(pixels);
                newTex.Apply();

                byte[] pngData = newTex.EncodeToPNG();
                System.IO.File.WriteAllBytes(path, pngData);

                importer.isReadable = wasReadable;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Debug.Log($"Converted {obj.name} using top-left pixel RGB({chromaKey.r:F2},{chromaKey.g:F2},{chromaKey.b:F2}) as chroma key");
            }
        }
    }

    [MenuItem("Assets/Convert Top Left Pixel To Transparent Alpha", true)]
    public static bool ValidateConvertTopLeftPixelToTransparent()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                return true;
            }
        }
        return false;
    }
}
