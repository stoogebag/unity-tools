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
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class FaceAnimatedMixamoRig : MonoBehaviour
{
    //[SerializeField] private GameObject FaceSprites;

    [SerializeField] private List<GameObject> FaceRigPrefabs;
    
    [SerializeField]
    List<SalsaRigging> _faceRigs = new List<SalsaRigging>();

    private GameObject RigContainer;
    
    [SerializeField] private string ParentGOName = "mixamorig:Head";
    
    [SerializeField] private bool ClearRigs = true;
    [SerializeField] private float EyeRangeOfMotion = .01f;

    [SerializeField] private RuntimeAnimatorController animatorController;
    
    
    
    [Button]
    void RigFaces()
    {
        RigContainer = gameObject.FirstOrDefault("SALSA FACE RIGS"); 
        if (ClearRigs)
        {
            if(RigContainer != null) DestroyImmediate(RigContainer);
        }

        var headBone = this.GetChild(ParentGOName);
        if(RigContainer == null) RigContainer = new GameObject("SALSA FACE RIGS");
        RigContainer.transform.SetParent(headBone.transform);

        var source = RigContainer.AddComponent<AudioSource>();
        
        foreach (var prefab in FaceRigPrefabs)
        {
            var sprites = Instantiate(prefab, RigContainer.transform);
            sprites.name = prefab.name;
           
            var rigger = sprites.AddComponent<SalsaRigging>();
            rigger.gameObject.TrimChildNames();
            rigger.Rig(source, EyeRangeOfMotion);

            _faceRigs.Add(rigger);
        }
        
        SetEmotion(_faceRigs.First().name);
    }

    [Button]
    void RigRagdoll()
    {
        
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
        var fbbik = gameObject.TryGetOrAddComponent<FullBodyBipedIK>();
        var grounder = gameObject.TryGetOrAddComponent<GrounderFBBIK>();
        var aim = gameObject.TryGetOrAddComponent<AimIK>();
        var lookIK = gameObject.TryGetOrAddComponent<LookAtIK>();
        _animator = gameObject.TryGetOrAddComponent<Animator>();

        _animator.runtimeAnimatorController = animatorController;
        _rigidBody = GetComponentInParent<Rigidbody>();

        //targets for hands and look, at least.
        var look = GetOrCreateIKTarget("look");
        var body = GetOrCreateIKTarget("body");
        
        fbbik.solver.leftHandEffector.target = GetOrCreateIKTarget("leftHand");
        fbbik.solver.leftFootEffector.target = GetOrCreateIKTarget("leftFoot");
        fbbik.solver.rightHandEffector.target = GetOrCreateIKTarget("rightHand");
        fbbik.solver.rightFootEffector.target = GetOrCreateIKTarget("rightFoot");

        lookIK.solver.target = look;
        
        var head =  gameObject.FirstOrDefault("mixamorig:Head").transform;
        var spine = gameObject.FirstOrDefault("mixamorig:Spine").transform;
        var spine1 = gameObject.FirstOrDefault("mixamorig:Spine1").transform;
        var spine2 = gameObject.FirstOrDefault("mixamorig:Spine2").transform;
        var neck = gameObject.FirstOrDefault("mixamorig:Neck").transform;
        var hips = gameObject.FirstOrDefault("mixamorig:Hips").transform;


        lookIK.solver.SetChain(new []{spine, spine1,spine2,neck}, head, null, hips);
        
        fbbik.solver.bodyEffector.target = body;
    }

    [SerializeField]
    private Transform IKTargetsContainer;
    
    Transform GetOrCreateIKTarget(string name)
    {
        var target = IKTargetsContainer.Find(name);
        if (target == null)
        {
            var go = new GameObject(name);
            target = go.transform;
            target.SetParent(IKTargetsContainer.transform);
        }

        return target;
    }

    private void FixedUpdate()
    {
        float speed;
        if(_animator == null) _animator = GetComponent<Animator>();
        if (AgentType == AgentTypes.RigidBody)
        {
            if(_rigidBody == null) _rigidBody = GetComponentInParent<Rigidbody>();
            speed = _rigidBody.velocity.magnitude;
        }
        else if (AgentType == AgentTypes.Navmesh)
        {
            if(_navMeshAgent == null) _navMeshAgent = GetComponentInParent<NavMeshAgent>();
            speed = _navMeshAgent.velocity.magnitude;
        }
        else
        {
            speed = 0;
        }
        //if(_cc == null) _cc = GetComponentInAncestor<MainCharacterControllerInControl>();
        _animator.SetFloat("Speed",speed, 0.1f,Time.deltaTime);
        _animator.SetBool("Crouched", Crouch);
    }
    public bool Crouch { get; set; }

    public AgentTypes AgentType;
    private NavMeshAgent _navMeshAgent;

    public enum AgentTypes
    {
        Navmesh,
        RigidBody,
        Other,
    }
}
  


#endif