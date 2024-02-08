using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using System.IO;

public class BatchImportAssetPackages : ScriptableWizard
{
    public string packagePath = "";

    [MenuItem("Game Gen/Asset/Import Packages")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Menu", typeof(BatchImportAssetPackages));
    }

    void OnWizardCreate()
    {
        packagePath = packagePath.Replace("\\", "/") + "/";

        string[] allFilePaths = Directory.GetFiles(Path.GetDirectoryName(packagePath));

        try
        {
            foreach (string curPath in allFilePaths)
            {
                string fileToImport = curPath.Replace("\\", "/");
                if (Path.GetExtension(fileToImport).ToLower() == ".unitypackage")
                {
                    Debug.Log("Importing: " + fileToImport);
                    AssetDatabase.ImportPackage(fileToImport, false);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
    }
}