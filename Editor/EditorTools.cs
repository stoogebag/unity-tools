#if UNITY_EDITOR
using UnityEditor;

namespace stoogebag.Editor
{
    public static class EditorTools {

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
                    AssetDatabase.CreateFolder(partial,split[i]);

                partial = newPartial;
            }
        } 
        
        
        
        
    }
}

#endif