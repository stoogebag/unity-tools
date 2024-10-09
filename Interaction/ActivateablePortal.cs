#if CINEMACHINE && UNITASK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;



public class ActivateablePortal : Activateable
{
    private void Awake()
    {
        Cables = FindObjectsOfType<Cable>().Where(t => t.Child == this).ToList();
    }

    public override void OnParentPowered()
    {
        if (Cables.All(t => t.Powered == PoweredState.Powered))
        {
            var inPortal = GetComponentsInChildren<Portal>(true).First(t => t.name == "in");

            inPortal.GetComponent<MeshRenderer>().enabled = true;
            inPortal.GetComponent<Collider>().enabled = true;

        }
        
    }
}
#endif