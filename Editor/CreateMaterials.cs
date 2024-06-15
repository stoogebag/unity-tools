using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
using stoogebag.Editor;
using stoogebag.Extensions;

public class CreateMaterials : ScriptableWizard
{
    [SerializeField] string Folder = "Assets/Materials";
    [SerializeField] string ShaderName = "Universal Render Pipeline/Lit";

    [SerializeField] private Shader Shader;// = Shader.Find(ShaderName);
    
    [SerializeField] bool createSubfolder = true;
    [SerializeField] bool RandomiseColours = true;

    [SerializeField] bool UseChildNames = true;



    [MenuItem("GameObject/Create and Assign Materials From Meshes")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CreateMaterials>("Create Materials", "Create");
    }

    //
    // [UnityEditor.MenuItem("Assets/test")]
    // static void CreateAssetWizard()
    // {
    //     ScriptableWizard.DisplayWizard<CreateMaterials>("Create Materials", "Create");
    // }


    void OnWizardCreate()
    {
        var go = Selection.activeGameObject;

        if (go == null) return;

        try{

        var name = Selection.activeGameObject.name;
        //var path = Folder + "/" + name;

        int count = 0;

        go.ForAllChildrenRecursive(child =>
        {
            if (child.TryGetComponent<MeshRenderer>(out var renderer))
            {
                count++;
                var folderPath = Folder + (createSubfolder ? "/" + go.name : "");
                var matName =  name + "_" + (UseChildNames ? child.name + "_" : "") + count;
                var path = folderPath + "/" + matName + ".mat";

                
                if (renderer.sharedMaterial.name != "Lit" && 
                    renderer.sharedMaterial.name != "Universal Render Pipeline/Lit" &&
                    renderer.sharedMaterial.name != "Default-Material")
                 {
                     return;
                 }
                
                var mat = new Material(Shader.Find(ShaderName));


                if (RandomiseColours) mat.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

                EditorTools.CreateFolder(folderPath);
                AssetDatabase.CreateAsset(mat, path);

                renderer.material = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));

            }
            else if (child.TryGetComponent<SkinnedMeshRenderer>(out var skinnedRenderer))
            {
                count++;
                var folderPath = Folder + (createSubfolder ? "/" + go.name : "");
                var matName = name + "_" + (UseChildNames ? child.name + "_" : "") + count;
                var path = folderPath + "/" + matName + ".mat";

                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                if (RandomiseColours) mat.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

                EditorTools.CreateFolder(folderPath);
                AssetDatabase.CreateAsset(mat, path);

                skinnedRenderer.material = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));

            }


        });        
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

}