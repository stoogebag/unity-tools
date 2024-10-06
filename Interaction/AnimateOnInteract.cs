#if CINEMACHINE && UNITASK
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class AnimateOnInteract : PointOfInterest
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

        GetComponent<Interactable>().OnInteractionObservable.Subscribe(OnTryInteract);
        

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


    public void OnTryInteract(IInteractor interactor)
    {
        
        var canInteract = GetComponent<ICondition>()?.GetValue() ?? true;
        if (!canInteract) return;
        SetActiveState(!Active.Value.Value);
    }

    [Button]
    public async UniTask SetActiveState(bool b)
    {
        await Active.SetValueAwaitable(b);
    }

    public void OnInteractionCancelled(IInteractor interactor)
    {
        
    }

    public void OnInteraction(IInteractor interactor)
    {
    }
}

#endif