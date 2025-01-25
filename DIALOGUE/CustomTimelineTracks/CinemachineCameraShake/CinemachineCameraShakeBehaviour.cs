using System;
using Cinemachine;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;

//WEIGHT GIVES NORMALISED TIME!!!

[Serializable]
public class CinemachineCameraShakePlayableBehaviour : PlayableBehaviour
{

    public CinemachineVirtualCamera vc;
    
    public float shakeAmplitude = 1f;
    
    public override void OnPlayableCreate (Playable playable)
    {
        
    }



    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //playerData = playerData as GameObject;

        vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * info.weight;
        
       
    }


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }
}
