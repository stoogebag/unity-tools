

using System;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class Extensions
    {
#if UNITY_EDITOR
        public static T Instantiate<T>(this MonoScript s) where T : ScriptableObject
        {
            return (T)ScriptableObject.CreateInstance(s.GetClass());
        }

#endif



        public static T GetRandomEnumValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
    }
}
