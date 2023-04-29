#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class Extensions
    {

        public static T Instantiate<T>(this MonoScript s) where T:ScriptableObject
        {
            return  (T)ScriptableObject.CreateInstance(s.GetClass());
        }
    
    
    }
}

#endif