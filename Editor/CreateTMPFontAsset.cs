using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;

public class CreateTMPFontAsset
{
    [MenuItem("Assets/stooge/Create TMP Font Asset", true)]
    static bool Validate()
    {
        foreach (var obj in Selection.objects)
            if (obj is Font) return true;
        return false;
    }

    [MenuItem("Assets/stooge/Create TMP Font Asset", false, 2000)]
    static void Run()
    {
        List<TMP_FontAsset> created = new List<TMP_FontAsset>();
        int skipped = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var obj in Selection.objects)
            {
                if (!(obj is Font font))
                {
                    skipped++;
                    continue;
                }

                TMP_FontAsset fa = CreateAsset(font);
                if (fa != null) created.Add(fa);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.SaveAssets();
        Selection.objects = created.ToArray();
        EditorUtility.FocusProjectWindow();

        if (created.Count > 0)
            Debug.Log($"Created {created.Count} TMP font asset{(created.Count > 1 ? "s" : "")}: {string.Join(", ", created.ConvertAll(f => f.name))}");
        if (skipped > 0)
            Debug.LogWarning($"Skipped {skipped} selected asset{(skipped > 1 ? "s" : "")} that {(skipped > 1 ? "are" : "is")} not a Font.");
    }

    static TMP_FontAsset CreateAsset(Font font)
    {
        string dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(font));
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, font.name + " SDF.asset"));

        TMP_FontAsset fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
        if (fa == null) { Debug.LogError("TMP asset failed for " + font.name); return null; }

        AssetDatabase.CreateAsset(fa, assetPath);
        if (fa.material != null) AssetDatabase.AddObjectToAsset(fa.material, fa);
        if (fa.atlasTextures != null && fa.atlasTextures.Length > 0 && fa.atlasTextures[0] != null)
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);

        return fa;
    }
}
