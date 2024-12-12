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


public class AnimateOnActivate : Activateable
{
    private DelayedBool Active;

    [SerializeField] private GameObject inactivePose;
    [SerializeField] private GameObject activePose;
    [SerializeField] private GameObject GameObjectToAnimate;

    PosRot inactivePosRot;
    PosRot activePosRot;
    
    public float ActivateTime = 1f;
    public float DeactivateTime = .2f;
    
    private void Awake()
    {
        inactivePosRot = new PosRot(inactivePose.transform, true);
        activePosRot = new PosRot(activePose.transform, true);

        

        Func<Tween> ATFunc = () =>
        {
            var activateTween = DOTween.Sequence().SetAutoKill(false).Pause();
            activateTween.Insert(0,
                GameObjectToAnimate.transform.DOLocalMove(activePosRot.Position, ActivateTime).SetAutoKill(false)
                    .Pause());
            activateTween.Insert(0,
                GameObjectToAnimate.transform.DOLocalRotate(activePosRot.Rotation.eulerAngles, ActivateTime)
                    .SetAutoKill(false).Pause());
            return activateTween;
        };
        Func<Tween> DTFunc = () =>
        {
            var deactivateTween = DOTween.Sequence().SetAutoKill(false).Pause();
            deactivateTween.Insert(0,
                GameObjectToAnimate.transform.DOLocalMove(inactivePosRot.Position, DeactivateTime).SetAutoKill(false)
                    .Pause());
            deactivateTween.Insert(0,
                GameObjectToAnimate.transform.DOLocalRotate(inactivePosRot.Rotation.eulerAngles, DeactivateTime)
                    .SetAutoKill(false).Pause());
            return deactivateTween;
        };
        Active = new DelayedBool(ATFunc, DTFunc, 0, false);
    }


    [Button]
    public async UniTask SetActiveState(bool b)
    {
        await Active.SetValueAwaitable(b);
    }


    public override void OnParentPowered()
    {
        Active.SetValue(Cables.All(t=>t.Powered == PoweredState.Powered));
    }

    public override void OnParentUnpowered()
    {
        Active.SetValue(Cables.All(t=>t.Powered == PoweredState.Powered));
    }
}

public struct PosRot
{
    public Vector3 Position;
    public Quaternion Rotation;

    public PosRot(Transform transform, bool useLocal = false)
    {
        if (useLocal)
        {
            Position = transform.localPosition;
            Rotation = transform.localRotation;
        }
        else
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }
    }
}


public interface ICondition
{
    public bool GetValue();
}
#endif