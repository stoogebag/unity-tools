
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Febucci.TextAnimatorCore;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UniRx;
using UnityEngine;

public class DialoguePanel : Window, IInitializes
{
    public DialogueSpeaker Speaker;
    [SerializeField] TypewriterComponent textTypewriter;
    [SerializeField] TypewriterComponent labelTypewriter;
    [SerializeField] Window nextIndicator;
    [SerializeField] AudioSource audioSource;

    private CompositeDisposable disposables = new CompositeDisposable();

    BoolReactiveProperty activated = new BoolReactiveProperty(false);
    private float timeSinceActivationChanged = 100;


    private void Awake()
    {
        Initialize();
        //textAnimator = textTypewriter.GetComponent<TextAnimator_TMP>();
        //labelAnimator = labelTypewriter.GetComponent<TextAnimator_TMP>();
    }

    public void Initialize()
    {
        disposables.Clear();
        Speaker = GetComponentInParent<DialogueSpeaker>(); //should i just have the user assign this?


        DialogueBehaviour.DialogueTriggeredObservable.Subscribe(async dialogue =>
        {
            if (Speaker == null) return;
            if (dialogue.speakerName != Speaker.Name) return;
            
            // var skippable = SkippableTimeline.CurrentlyPlayingTimeline;
            // if (skippable != null)
            //     skippable.TypingTypewriter = textTypewriter;
                
            await Show(dialogue);
            
            // if (skippable != null)
            //     skippable.TypingTypewriter = null;
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
            //BC: there was a reason this was here. probably related to dialogue system usage. keep an eye out
        //    if (activated.Value == false) Hide(); 
        }
        timeSinceActivationChanged += Time.deltaTime;
        
         if(!textTypewriter.IsShowingText && Active == ActiveState.Active)
             nextIndicator?.Activate();
        
    }

    private async UniTask Show(DialogueBehaviour dialogue)
    {
        activated.Value = true;
        if(nextIndicator != null) nextIndicator.DeactivateImmediate();

        Activate().Forget();

        if (dialogue.Clip != null && audioSource != null)
            audioSource.PlayOneShot(dialogue.Clip);
        
        if (labelTypewriter != null)
        {
            if (labelTypewriter.TextAnimator.textFull != dialogue.speakerName)
                labelTypewriter.ShowTextAndAwait(dialogue.speakerName).Forget();
        }

        await textTypewriter.ShowTextAndAwait(dialogue.dialogueLine);
    }

    public async UniTask Bark(DialogueLine line, string speakerName, float lingerTime = 1f, float fadeInTime = 0.1f, float fadeOutTime = 1f)
    {
        await Bark(line.Text, speakerName, lingerTime, fadeInTime, fadeOutTime);
    }

    
    public async UniTask Bark(string message, string speakerName = null, float lingerTime = 1f, float fadeInTime = 0.1f, float fadeOutTime = 1f)
        {

            Activate().Forget();
            if(labelTypewriter.TextAnimator.textFull != speakerName)
                labelTypewriter.ShowTextAndAwait(speakerName).Forget();

            await textTypewriter.ShowTextAndAwait(message); // assume the longest task is the text writing...
            await UniTask.WaitForSeconds(lingerTime);
            await Deactivate();
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
