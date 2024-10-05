using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class HoudiniBind : MonoBehaviour
{
    [SerializeField]
    private GameObject myGameObject;
    
    [Button]
    void Bind()
    {
        var houdini = GetComponentInChildren<HoudiniEngineUnity.HEU_HoudiniAssetRoot>();
        if (houdini == null)
        {
            Debug.LogError("No Houdini Asset Root found");
            return;
        }

        houdini.HoudiniAsset.RequestReload();
        
        var node =houdini.HoudiniAsset.InputNodes[0];

        
        var colliders = myGameObject.GetComponentsInDescendants<Collider>(true);

        
        print("gameobject: " + gameObject.name);
        
        var index = 0;
        foreach (var c in colliders)
        {
            c.name += " " + Guid.NewGuid().ToString();
            if(node.NumInputEntries() == index) 
                node.AddInputEntryAtEnd(c.gameObject);
            else 
                node.SetInputEntry(index,c.gameObject, true);

            index++;
        }
        
        houdini.HoudiniAsset.RequestReload(true);

    }
}
