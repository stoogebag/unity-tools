#if  UNITY_EDITOR


using System;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Editor
{
    public class EditorMenus
    {
        [MenuItem("GameObject/Save As Prefab")]
        public static void SaveAsPrefab()
        {
            var gameObject = Selection.activeGameObject;
            SaveAsPrefab(gameObject);
        }

        public static void SaveAsPrefab(GameObject gameObject)
        {
            var folderPath = "Assets/Prefabs/Saved";
            EditorTools.CreateFolder(folderPath);
            var path = folderPath + "/" + gameObject.name + "-" + DateTime.Now.ToString("hh-mm-ss") + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        }

        [MenuItem("stooge/Autorefresh Off")]
        public static void AutorefreshOff()
        {
            EditorPrefs.SetBool("kAutoRefresh", false);
        }

        [MenuItem("stooge/Autorefresh On")]
        public static void AutorefreshOn()
        {
            EditorPrefs.SetBool("kAutoRefresh", true);
        }

    }
}


#endif