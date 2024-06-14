#if UI_SHAPES_KIT
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using ThisOtherThing.UI.Shapes;
using UniRx;
using UnityEngine;

public class ButtonInteractable : PointOfInterest, IInteractable
{
    public UIPopup Popup => gameObject.FirstOrDefault<UIPopup>();
    private Arc arc => gameObject.FirstOrDefault<Arc>();
    [SerializeField] private float CooldownInSeconds = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Popup = GetComponentInDescendent<UIPopup>();

        EndCooldown();

        OnActivate.OnActivate += () =>
        {
            print("activate");
            Activated();
            arc.ArcProperties.Length = 0f;
            arc.ForceMeshUpdate();
            BeginCooldown();
        };
    }

    private void Activated()
    {
        OnActivate.SetValue(false);
        print("!");
    }

    private async void BeginCooldown()
    {
        print("cooldown started.");

        OnActivate.onCancel = null;
        OnActivate.onTry = null;
        
        _active = false;
        await UniTask.WaitForSeconds(CooldownInSeconds);
        EndCooldown();
    }
    private void EndCooldown()
    {
        print("cooldown done.");
        OnActivate.onCancel = DOVirtual.Float(arc.ArcProperties.Length, 0f, CancelAnimationDuration, value =>
            {
                arc.ArcProperties.Length = value;
                arc.ForceMeshUpdate();
            } )
            .SetEase(Ease.Linear)
            .SetAutoKill(false)
            .Pause();
        
        var onTrue = DOTween.Sequence();
        //onTrue.AppendInterval(0.1f);
        //onTrue.Append(transform.DOLocalMoveY(-.5f, 1f));
        onTrue.Append(DOVirtual.Float( arc.ArcProperties.Length, 1, ActivationTime, value =>
        {
            arc.ArcProperties.Length = value;
            arc.ForceMeshUpdate();
        }));
        
        onTrue.SetAutoKill(false)
            .SetEase(Ease.Linear);
        onTrue.Pause();

        OnActivate.onTry = onTrue;
        
        _active = true;
    }


    public DelayedAction OnActivate = new DelayedAction();

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFocus(IInteractor interactor)
    {
        Popup.Activate();
    }

    public string InteractText { get; set; } = "Button";

    public void OnUnfocus(IInteractor interactor)
    {
        Popup.Disable();
    }

    public void OnTryInteract(IInteractor interactor)
    {
        //time lag.
        if(_active) OnActivate.SetValue(true);
    }
    

    public void OnInteractionCancelled(IInteractor interactor)
    {
        OnActivate.SetValue(false);
    }
    
    public void OnInteraction(IInteractor interactor)
    {
        Pressed = !Pressed;
    }

    public bool Pressed;

    public float InteractionDelay = 1f;
    public float InteractionProgress;
    [SerializeField] private bool _active;
    [SerializeField] public float ActivationTime = 1f;
    [SerializeField] private float CancelAnimationDuration = 1f;

}

#endif