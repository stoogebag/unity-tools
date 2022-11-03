
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Editor
{
    [ExecuteInEditMode]
    public class RemoveEmptyScripts : MonoBehaviour
    {
        void Start()
        {
        
        }

        void Update()
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        }
    }
}
#endif