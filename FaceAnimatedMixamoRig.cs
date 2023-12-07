#if SALSA

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stoogebag._2dConvos;
using stoogebag.Extensions;
using UnityEngine;

public class FaceAnimatedMixamoRig : MonoBehaviour
{
    [SerializeField] private GameObject FacePrefab;

    [SerializeField]
    private SalsaRigging _faceRig;

    [SerializeField] private string ParentGOName = "mixamorig:Head";
    
    
    [Button]
    void Rig()
    {
        if(_faceRig != null) DestroyImmediate(_faceRig.gameObject);

        var headBone = this.GetChild(ParentGOName);
        var rig = Instantiate(FacePrefab, headBone.transform);

        var rigger = rig.AddComponent<SalsaRigging>();

        rigger.gameObject.TrimChildNames();
        rigger.Rig();

        _faceRig = rigger;

    } 


}
#endif