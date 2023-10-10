using System.Linq;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class InputExtensions
    {

        public static bool GetKeyComboDown(params KeyCode[] keys)
        {
            return GetKeyCombo(keys) && keys.Any(k => UnityEngine.Input.GetKeyDown(k));
        }
        
        public static bool GetKeyCombo(params KeyCode[] keys)
        {
            foreach (var key in keys)
            {
                if (!UnityEngine.Input.GetKey(key)) return false;
            }

            return true;
        }


    }
}