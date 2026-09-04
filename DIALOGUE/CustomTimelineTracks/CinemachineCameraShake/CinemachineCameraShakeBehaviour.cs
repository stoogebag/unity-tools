using System;

#if NEW_CINEMACHINE
using Unity.Cinemachine;
#else
using Cinemachine;
#endif
using UnityEngine.Playables;

//WEIGHT GIVES NORMALISED TIME!!!

[Serializable]
public class CinemachineCameraShakePlayableBehaviour : PlayableBehaviour
{
    public CinemachineVirtualCamera vc;

    public float shakeAmplitude = 1f;

    public override void OnPlayableCreate(Playable playable)
    {
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //playerData = playerData as GameObject;

#if NEW_CINEMACHINE
        vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = shakeAmplitude * info.weight;
#else
        vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * info.weight;
#endif
    }


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }
}