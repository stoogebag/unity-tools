using System.Collections.Generic;
using UnityEngine;

public class HoudiniTag : MonoBehaviour
{
    [Header("Houdini Export")]
    public List<string> tags;
    public string guid;
    public GameObject productionPrefab;

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
