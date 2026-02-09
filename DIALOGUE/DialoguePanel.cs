
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Febucci.TextAnimatorCore;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UniRx;
using UnityEngine;

public class DialoguePanel : Window, IInitializes
{
    public DialogueSpeaker Speaker;
    [SerializeField] TextAnimator_TMP textTypewriter;
    [SerializeField] TextAnimator_TMP labelTypewriter;
    [SerializeField] Window nextIndicator;

    private CompositeDisposable disposables = new CompositeDisposable();

    BoolReactiveProperty activated = new BoolReactiveProperty(false);
    private float timeSinceActivationChanged = 100; 
    
    
    public void Initialize()
    {
        
        disposables.Clear();
        Speaker = GetComponentInParent<DialogueSpeaker>(); //should i just have the user assign this?
        //typewriter = GetComponentInChildren<TypewriterCore>(true);

        DialogueBehaviour.DialogueTriggeredObservable.Subscribe(async dialogue =>
        {
            if (Speaker == null) return; //bc: wtf is happening here? stale subs? but i dispose it all TT. could it be because of static
            if (dialogue.speakerName != Speaker.Name) return;
            
            var skippable = dialogue.director.GetComponent<SkippableTimeline>(); //not sure about this.
            skippable.TypingTypewriter = textTypewriter;
            await Show(dialogue);
            skippable.TypingTypewriter = null;


        }).AddTo(disposables);
        DialogueBehaviour.DialogueEndedObservable.Subscribe(dialogue =>
        {
            if (Speaker == null) return; //bc: wtf is happening here? stale subs? but i dispose it all TT. could it be because of static
            if (dialogue.speakerName != Speaker.Name) return;
            activated.Value = false;
        }).AddTo(disposables);

        activated.Subscribe(val =>
        {
            timeSinceActivationChanged = 0;
        });
    }

    private void Update()
    {
        if (timeSinceActivationChanged > 0.05f)
        {
            if (activated.Value == false) Hide();
        }
        timeSinceActivationChanged += Time.deltaTime;
        
         if(textTypewriter.allLettersShown && Active == ActiveState.Active)
             nextIndicator?.Activate();
        
    }

    private async UniTask Show(DialogueBehaviour dialogue)
    {
        activated.Value = true;
        if(nextIndicator != null) nextIndicator.DeactivateImmediate();

        Activate().Forget();

        
        //todo: make it happen
        textTypewriter.ShowTextAndAwait(dialogue.dialogueLine).Forget();
        if(labelTypewriter.GetComponent<TextAnimator_TMP>().textFull != dialogue.speakerName)
            labelTypewriter.ShowTextAndAwait(dialogue.speakerName).Forget();

        //await UniTask.WhenAll(textTypewriter.ShowTextAndAwait(dialogue.dialogueLine), Activate());
        

    }
    
    private async void Hide(float delay = 0.1f)
    {
         await Deactivate();
    }
    

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}

public interface IInitializes 
{
    void Initialize();
}
