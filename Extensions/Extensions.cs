using UnityEditor;
using UnityEngine;

public static class Extensions
{

    public static T Instantiate<T>(this MonoScript s) where T:ScriptableObject
    {
        return  (T)ScriptableObject.CreateInstance(s.GetClass());
    }
    
    
}