#if SALSA

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using stoogebag._2dConvos;
using stoogebag.Extensions;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class FaceAnimatedMixamoRig : MonoBehaviour
{
    //[SerializeField] private GameObject FaceSprites;

    [SerializeField] private List<GameObject> RigPrefabs;
    
    [SerializeField]
    List<SalsaRigging> _faceRigs = new List<SalsaRigging>();

    private GameObject RigContainer;
    
    [SerializeField] private string ParentGOName = "mixamorig:Head";
    
    [SerializeField] private bool ClearRigs = true;

    [SerializeField] private AnimatorController animatorController;
    
    [Button]
    void RigFaces()
    {
        if (ClearRigs)
        {
            if(RigContainer != null) DestroyImmediate(RigContainer);
        }

        var headBone = this.GetChild(ParentGOName);
        if(RigContainer == null) RigContainer = new GameObject("SALSA FACE RIGS");
        RigContainer.transform.SetParent(headBone.transform);

        var source = RigContainer.AddComponent<AudioSource>();
        
        foreach (var prefab in RigPrefabs)
        {
            var sprites = Instantiate(prefab, RigContainer.transform);
            sprites.name = prefab.name;
           
            var rigger = sprites.AddComponent<SalsaRigging>();
            rigger.gameObject.TrimChildNames();
            rigger.Rig(source);

            _faceRigs.Add(rigger);
        }
        
        SetEmotion(_faceRigs.First().name);
    }


    private List<string> Emotions;
    public string CurrentEmotion;
    private Animator _animator;
    private Rigidbody _rigidBody;

    [Button]
    public void SetEmotion(string s)
    {
        CurrentEmotion = s;
        foreach (var rig in _faceRigs)
        {
            if(rig.name == CurrentEmotion) rig.gameObject.SetActive(true);
            else rig.gameObject.SetActive(false);
        }
    }

    [Button]
    void RigBody()
    {
        var fbbik = gameObject.ReplaceOrAddComponent<FullBodyBipedIK>();
        var grounder = gameObject.ReplaceOrAddComponent<GrounderFBBIK>();
        _animator = gameObject.ReplaceOrAddComponent<Animator>();

        _animator.runtimeAnimatorController = animatorController;
        _rigidBody = GetComponentInParent<Rigidbody>();
    }


    private void Update()
    {
        if(_rigidBody == null) _rigidBody = GetComponentInParent<Rigidbody>();
        if(_animator == null) _animator = GetComponent<Animator>();
        _animator.SetFloat("Speed",_rigidBody.velocity.magnitude);
    }
}
  


#endif