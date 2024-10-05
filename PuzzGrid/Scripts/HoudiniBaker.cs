using System.Collections;
using System.Collections.Generic;
using HoudiniEngineUnity;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class HoudiniBaker : MonoBehaviour
{
    [Button]
    public void Bake()
    {
        //HDA.HoudiniAsset.InputNodes.Clear();
        
        //HDA.HoudiniAsset.InputNodes.Add(new HEU_InputNode());
        
        
        HDA.HoudiniAsset.RequestReload();
        HDA.HoudiniAsset.GetInputNodeByIndex(0).RemoveAllInputEntries();
        HDA.HoudiniAsset.GetInputNodeByIndex(0).AddInputEntryAtEnd(houdiniInputs);
        
        var node = HDA.HoudiniAsset.GetInputNodeByIndex(0);
        //node.InputName = InputName;
        
        HDA.HoudiniAsset.RequestCook(false, false, true, true);

        Outputs.DestroyAllImmediateAndClear();

        var output = HDA.HoudiniAsset.GetOutputGameObjects(Outputs);
        foreach (var go in Outputs)
        {
            go.transform.parent = houdiniOutputContainer.transform;
        }

    }

    [SerializeField] private List<GameObject> Outputs;
    
    [SerializeField] private HEU_HoudiniAssetRoot HDA;
    [SerializeField] private GameObject houdiniInputs;
    [SerializeField] private GameObject  houdiniOutputContainer;

    [SerializeField] private string InputName = "blockout shapes";




}
