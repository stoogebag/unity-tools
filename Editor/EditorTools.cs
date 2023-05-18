
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace stoogebag.Editor
{
    public static class EditorTools
    {

        //[MenuItem("stoogebag/Tools/Install Package/monKey")]
        //static void InstallMonKey() => Packages.InstallUnityPackage("monkey");

        public static void CreateFolder(string path)
        {
            var split = path.Split('/');
            var partial = split[0];

            for (var i = 1; i < split.Length; i++)
            {
                //this is gross but whatever trevor
                //this assumes Assets exists, i think that is reasonable

                var newPartial = partial + "/" + split[i];

                if (!AssetDatabase.IsValidFolder(newPartial))
                    AssetDatabase.CreateFolder(partial, split[i]);

                partial = newPartial;
            }
        }
    }


    [System.Serializable]
     public class SceneField
     {
         [SerializeField] private Object sceneAsset;
         [SerializeField] private string sceneName = "";
 
         public string SceneName
         {
             get { return sceneName; }
         }
 
         // makes it work with the existing Unity methods (LoadLevel/LoadScene)
         public static implicit operator string(SceneField sceneField)
         {
             return sceneField.SceneName;
         }
     }
 
 #if UNITY_EDITOR
     [CustomPropertyDrawer(typeof(SceneField))]
     public class SceneFieldPropertyDrawer : PropertyDrawer
     {
         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
         {
             EditorGUI.BeginProperty(position, GUIContent.none, property);
             var sceneAsset = property.FindPropertyRelative("sceneAsset");
             var sceneName = property.FindPropertyRelative("sceneName");
             position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
             if (sceneAsset != null)
             {
                 EditorGUI.BeginChangeCheck();
                 var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                 if (EditorGUI.EndChangeCheck())
                 {
                     sceneAsset.objectReferenceValue = value;
                     if (sceneAsset.objectReferenceValue != null)
                     {
                         var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                         var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                         var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                         scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                         sceneName.stringValue = scenePath;
                     }
                 }
             }
             EditorGUI.EndProperty();
         }
     }
 #endif
 }
        
    
    
    

