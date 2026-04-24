#if SPLINES

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Splines;

//WEIGHT GIVES NORMALISED TIME!!!

[Serializable]
public class SplineAnimatePlayableBehaviour : PlayableBehaviour
{

    public SplineContainer splineContainer;
    
    public override void OnPlayableCreate (Playable playable)
    {
        
    }



    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //playerData = playerData as GameObject;
        _splineAnimate = (playerData as SplineAnimate);

        if (_splineAnimate == null)
        {
            Debug.LogError("SplineAnimatePlayableBehaviour: SplineAnimate is null!");
            return;
        }

        if (info.weight > 0)
        {
            _splineAnimate.Container = splineContainer;
            _splineAnimate.Play();
            
            _splineAnimate.NormalizedTime = info.weight;
            
        }
        else _splineAnimate.Pause();
        //Debug.Log( (float)playable.GetTime());
    }

    private SplineAnimate _splineAnimate;

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        _splineAnimate?.Pause();
    }
}
#endif