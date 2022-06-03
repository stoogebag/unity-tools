using System.Collections.Generic;
using UnityEngine;

namespace stoogebag
{
    public static class GameObjectExtensions { 
        //public static string TryGetGuid(this GameObject me)
        //{
        //    var gc = me.GetComponent<GuidComponent>();
        //}
        public static void DestroyAllAndClear(this List<GameObject> me)
        {
            foreach (var go in me)
            {
                UnityEngine.GameObject.Destroy(go);
            }
            me.Clear();
        }
    
    }
}
