
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
    [SerializeField] private float inactivityTimeout = 4f;

    private CompositeDisposable disposables = new CompositeDisposable();

    BoolReactiveProperty activated = new BoolReactiveProperty(false);
    private float timeSinceActivationChanged = 100;
    private float _inactivityTimer;


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


        DialogueTimelineBehaviour.DialogueTriggeredObservable.Subscribe(async dialogue =>
        {
            if (Speaker == null) return;
            if (dialogue.Speaker != Speaker) return;

            await Show(dialogue);
        }).AddTo(disposables);
        DialogueTimelineBehaviour.DialogueEndedObservable.Subscribe(dialogue =>
        {
            if (Speaker == null) return;
            if (dialogue.Speaker != Speaker) return;
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

        if (textTypewriter.IsShowingText)
        {
            _inactivityTimer = 0f;
        }
        else
        {
            _inactivityTimer += Time.deltaTime;
            if (_inactivityTimer >= inactivityTimeout && Active == ActiveState.Active)
            {
                Deactivate().Forget();
                _inactivityTimer = 0f;
            }
        }
        
         if(!textTypewriter.IsShowingText && Active == ActiveState.Active)
             nextIndicator?.Activate();
        
    }

    private async UniTask Show(DialogueTimelineBehaviour dialogue)
    {
        activated.Value = true;
        _inactivityTimer = 0f;
        if(nextIndicator != null) nextIndicator.DeactivateImmediate();

        Activate().Forget();

        var line = dialogue.Line;

        if (line.Clip != null && audioSource != null)
            audioSource.PlayOneShot(line.Clip);

        if (labelTypewriter != null)
        {
            var speakerName = line.Speaker != null ? line.Speaker.Name : string.Empty;
            if (labelTypewriter.TextAnimator.textFull != speakerName)
                labelTypewriter.ShowTextAndAwait(speakerName).Forget();
        }

        await textTypewriter.ShowTextAndAwait(line.Text ?? string.Empty);
    }

    public async UniTask Bark(DialogueLine line, string speakerName, float lingerTime = 1f, float fadeInTime = 0.1f, float fadeOutTime = 1f)
    {
        await Bark(line.Text, speakerName, lingerTime, fadeInTime, fadeOutTime);
    }

    
    public async UniTask Bark(string message, string speakerName = null, float lingerTime = 1f, float fadeInTime = 0.1f, float fadeOutTime = 1f)
        {

            _inactivityTimer = 0f;
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

    public override async UniTask Deactivate()
    {
        textTypewriter?.StopShowingText();
        textTypewriter?.StopDisappearingText();
        textTypewriter?.ShowText("");

        labelTypewriter?.StopShowingText();
        labelTypewriter?.StopDisappearingText();
        labelTypewriter?.ShowText("");

        await base.Deactivate();
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
