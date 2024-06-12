using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEditor;
using Sirenix.Utilities;
using UnityEditor.Animations;
using System.Linq;
using stoogebag.Extensions;
using UnityEngine.WSA;

public class SetLoopAndName : MonoBehaviour
{
    [MenuItem("Assets/set loop and name")]
    static void Execute()
    {
        var selection = Selection.gameObjects;


        foreach (var o in selection)
        {

            //              var assetPath = AssetDatabase.GetAssetPath(o);
            //var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            //var clip = importer.defaultClipAnimations[0];

            //clip.name = o.name;
            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(o));
            var clips = modelImporter.defaultClipAnimations;
            foreach (var c in clips)
            {
                if (c.name == "mixamo.com") c.name = o.name;
                c.loopTime = true;
            }
            modelImporter.clipAnimations = clips;
            //AnimationClip[] animations = AnimationUtility.GetAnimationClips(o);
        }


        //AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClipToChange);
        //        settings.loopTime = true;
        //        AnimationUtility.SetAnimationClipSettings(animClipToChange, settings);
    }

    [MenuItem("Assets/DoSomethingWithVariabe", true)]
    private static bool NewMenuOptionValidation()
    {
        // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
        //return Selection.activeObject is Variable;
        return true;
    }



    [MenuItem("Assets/create materials")]
    static void Materials()
    {

        //todo
        return;
        // var selection = Selection.gameObjects;
        //
        //
        // foreach (var o in selection)
        // {
        //
        //     //              var assetPath = AssetDatabase.GetAssetPath(o);
        //     //var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        //
        //     //var clip = importer.defaultClipAnimations[0];
        //
        //     //clip.name = o.name;
        //     ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(o));
        //     var clips = modelImporter.defaultClipAnimations;
        //     foreach (var c in clips)
        //     {
        //         if (c.name == "mixamo.com") c.name = o.name;
        //         c.loopTime = true;
        //     }
        //     modelImporter.clipAnimations = clips;
        //     //AnimationClip[] animations = AnimationUtility.GetAnimationClips(o);
        // }


        //AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClipToChange);
        //        settings.loopTime = true;
        //        AnimationUtility.SetAnimationClipSettings(animClipToChange, settings);
    }

    [MenuItem("Assets/create single material")]
    static void SingleMaterial()
    {
        var selection = Selection.gameObjects;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        var name = Selection.activeGameObject.name;
        var path = "Assets/" + name + ".mat";
        AssetDatabase.CreateAsset(mat, path);
        mat = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material)); //needed?
        
        foreach (var go in selection)
        {
            if (go == null) return;


            go.ForAllChildrenRecursive(child =>
            {
                if (child.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.sharedMaterial = mat;
                }
                else if (child.TryGetComponent<SkinnedMeshRenderer>(out var skinnedRenderer))
                {
                    // var folderPath = Folder + (createSubfolder ? "/" + go.name : "");
                    // var matName = name + "_" + (UseChildNames ? child.name + "_" : "") + count;
                    // var path = folderPath + "/" + matName + ".mat";
                    //
                    // var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    //
                    // if (RandomiseColours) mat.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                    //
                    // AssetDatabase.CreateAsset(mat, path);
                    //
                    // skinnedRenderer.material = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));

                    //todo!
                    
                }
            });
        }
    }



    [MenuItem("Assets/populate animator")]
    static void PopulateAnimator()
    {

        var selection = Selection.activeObject;



        if (selection is AnimatorController ac)
        {

            var path = AssetDatabase.GetAssetPath(selection);
            var folderPath = Path.GetDirectoryName(path);

            var idleClip = GetClipAtPath(folderPath, "Idle", new[]{"Idle", "mixamo.com"});//cringe
            var runClip = GetClipAtPath(folderPath, "Running",  new[]{"Running", "mixamo.com"});
            var walkClip = GetClipAtPath(folderPath, "Walking", new[]{"Walking", "mixamo.com"});
            var talkClip = GetClipAtPath(folderPath, "Talking", new[]{"Talking", "mixamo.com"});


            var movement = ac.layers[0];
            var talkState = ac.layers[0].stateMachine.states.First(t => t.state.name == "talking");
            talkState.state.motion = talkClip;



            var blendTree = ac.layers[0].stateMachine.states.First(t => t.state.name == "Blend Tree");


            var bt = (blendTree.state.motion as BlendTree);

            while (bt.children.Any()) bt.RemoveChild(0);

            bt.AddChild(idleClip, 0);
            bt.AddChild(walkClip, 2);
            bt.AddChild(runClip, 4);


        }

    }

    public static ModelImporter GetImporterAtPath(string folder, string name)
    {
        return ((ModelImporter)AssetImporter.GetAtPath(folder + "/" + name + ".fbx"));
    }
    public static AnimationClip GetClipAtPath(string folder, string name, string[] clipNames )
    {
        var array = AssetDatabase.LoadAllAssetsAtPath(folder + "/" + name + ".fbx");


        foreach (var o in array)
        {
            if (o is AnimationClip ac)
            {
                if (clipNames.Contains(ac.name))
                    return ac;
            }
        }

        return null;

    }
    [MenuItem("Assets/material from textures")]
    static void MatFromTexs()
    {
        var selection = Selection.activeObject;

        var path = AssetDatabase.GetAssetPath(selection);


        var assets = AssetDatabase.FindAssets("t:texture", new string[] { path });


        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        foreach(var a in assets)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(a);
            var tex = (Texture)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture));
            //mat.SetTexture("base map")

            if (assetPath.Contains("basecolor"))        mat.SetTexture("_BaseMap", tex);
            if (assetPath.Contains("normal"))           mat.SetTexture("_BumpMap", tex);
            if (assetPath.Contains("ambientocclusion")) mat.SetTexture("_OcclusionMap", tex);
            if (assetPath.Contains("metallic"))         mat.SetTexture("_MetallicGlossMap", tex);
            if (assetPath.Contains("height"))           mat.SetTexture("_ParallaxMap", tex);
            if (assetPath.Contains("roughness"))        mat.SetTexture("_BaseMap", tex);

        }

        var split = path.Split('/');
        mat.name = split[split.Length - 1];
        AssetDatabase.CreateAsset(mat, path + "/" + mat.name + ".mat");


    }
}