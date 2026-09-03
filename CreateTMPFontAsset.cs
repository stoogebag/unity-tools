using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;

public class CreateTMPFontAsset
{
    [MenuItem("Assets/stooge/Create TMP Font Asset", true)]
    static bool Validate() => Selection.activeObject is Font;

    [MenuItem("Assets/stooge/Create TMP Font Asset", false, 2000)]
    static void Run()
    {
        Font font = Selection.activeObject as Font;
        if (font == null) return;

        string dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(font));
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, font.name + " SDF.asset"));

        TMP_FontAsset fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
        if (fa == null) { Debug.LogError("TMP asset failed for " + font.name); return; }

        AssetDatabase.CreateAsset(fa, assetPath);
        if (fa.material != null) AssetDatabase.AddObjectToAsset(fa.material, fa);
        if (fa.atlasTextures != null && fa.atlasTextures.Length > 0 && fa.atlasTextures[0] != null)
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
        AssetDatabase.SaveAssets();
        Selection.activeObject = fa;
        EditorUtility.FocusProjectWindow();
    }
}
