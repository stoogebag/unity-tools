#if BEHAVIOR_DESIGNER
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyAgent : MonoBehaviour, ISoundListener
{
    public PatrolRoute PatrolRoute;
    private BehaviorTree _tree;


    private void Start()
    {
        _tree = GetComponent<BehaviorTree>();
        _tree.SetVariable("Patrol", PatrolRoute.PatrolPoints);
        
        //_lookTarget.transform.parent = transform; lol duh
    }

    [Button]
    void Shot(Vector3 origin)
    {
        var dir = (origin - transform.position).normalized;
        _lookTarget.Value = transform.position + dir;

        _poi.Value = origin;
        
        _tree.SetVariable("LookTarget", _lookTarget);
        _tree.SetVariable("PointOfInterest", _poi);
        _tree.SendEvent("Shot");
    }

    private SharedVector3 _lookTarget = new SharedVector3();
    private SharedVector3 _poi = new SharedVector3();
    
    
    public void TryHearSound(SoundEmission sound, Vector3 position, float loudness )
    {
        //todo: check if sound is loud enough to be heard or somthing
        
        var dir = (position - transform.position).normalized;
        _lookTarget.Value = transform.position + dir;
        
        var distance = (position - transform.position).magnitude;
        var apparentLoudness = HearingSensitivity * loudness / (distance * distance);

        Debug.Log($"{this.name} heard {sound.name} at volume {apparentLoudness}.");
        
        _poi.Value = position;
        _tree.SetVariable("PointOfInterest", _poi);
        _tree.SetVariable("PointOfInterest", _poi);
        _tree.SendEvent("HeardSound");
        
        
        
    }

    public float HearingSensitivity { get; } = 1f;

}


#endif