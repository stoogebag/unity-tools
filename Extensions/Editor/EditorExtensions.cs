using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace stoogebag.Editor
{
    public static class EditorExtensions
    {
        
        public static IEnumerable<T> GetAllInChildren<T>(this Object o)
        {
            var path = AssetDatabase.GetAssetPath(o);
            var ts = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<T>();

            foreach (var t in ts)
            {
                yield return t;
            }
        }
        
        
        
        
    }
}
#endif