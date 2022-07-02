
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UnityEditor;

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
#endif