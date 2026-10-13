using System;
using System.Collections.Generic;
using UnityEngine;

public enum HoudiniParamType { Int, Float, Bool, String }

[Serializable]
public struct HoudiniParam
{
    public string name;
    public HoudiniParamType type;
    public string value;
}

public class HoudiniTag : MonoBehaviour
{
    [Header("Houdini Export")]
    public List<string> tags;
    public string guid;
    public GameObject productionPrefab;

    [Header("Houdini Params")]
    public List<HoudiniParam> parameters;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
                return;
            guid = System.Guid.NewGuid().ToString();
        }
    }
#endif
}
