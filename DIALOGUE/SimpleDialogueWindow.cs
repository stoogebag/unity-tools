using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorForUnity;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDialogueWindow : Window, ITypewriterProgress
{
    [SerializeField] private TypewriterComponent textTypewriter;
    [SerializeField] private TypewriterComponent labelTypewriter;
    [SerializeField] private Image portrait;
    [SerializeField] private Window nextIndicator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool followSpeaker;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float advanceGraceSeconds = 0.3f;

    private Transform _follow;
    private bool _wasTyping;
    private float _revealFinishedAt = float.NegativeInfinity;
    private bool _revealFinishedByUser;
    private bool _pendingUserFill;

    private bool RawIsTyping => textTypewriter != null && textTypewriter.IsShowingText;

    public bool IsTyping => RawIsTyping;

    public void FinishTyping()
    {
        if (textTypewriter == null) return;
        _pendingUserFill = true;
        textTypewriter.SkipTypewriter();
    }

    public bool IsInAdvanceGrace
    {
        get
        {
            TrackReveal();
            if (RawIsTyping || _revealFinishedByUser) return false;
            return Time.unscaledTime - _revealFinishedAt < advanceGraceSeconds;
        }
    }

    private void TrackReveal()
    {
        var typing = RawIsTyping;
        if (_wasTyping && !typing)
        {
            _revealFinishedByUser = _pendingUserFill;
            _pendingUserFill = false;
            _revealFinishedAt = Time.unscaledTime;
        }

        _wasTyping = typing;
    }

    public async UniTask Show(DialogueLine line)
    {
        _wasTyping = false;
        _revealFinishedAt = float.NegativeInfinity;
        _revealFinishedByUser = false;
        _pendingUserFill = false;

        var speaker = line.Speaker;
        _follow = followSpeaker && speaker != null ? speaker.transform : null;

        if (labelTypewriter != null)
        {
            labelTypewriter.ShowText(speaker != null ? speaker.Name : string.Empty);
        }

        if (portrait != null)
        {
            var sprite = speaker != null ? speaker.Portrait : null;
            portrait.sprite = sprite;
            portrait.enabled = sprite != null;
        }

        if (line.Clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(line.Clip);
        }

        if (nextIndicator != null)
        {
            nextIndicator.DeactivateImmediate();
        }

        Activate().Forget();

        PlaybackControls.Instance?.SetTypewriter(this);

        await textTypewriter.ShowTextAndAwait(line.Text ?? string.Empty);

        if (nextIndicator != null)
        {
            nextIndicator.Activate().Forget();
        }
    }

    public async UniTask Hide()
    {
        _follow = null;
        _wasTyping = false;
        _revealFinishedAt = float.NegativeInfinity;
        _revealFinishedByUser = false;
        _pendingUserFill = false;

        PlaybackControls.Instance?.ClearTypewriter(this);

        if (textTypewriter != null)
        {
            textTypewriter.StopShowingText();
            textTypewriter.StopDisappearingText();
            textTypewriter.ShowText(string.Empty);
        }

        if (labelTypewriter != null)
        {
            labelTypewriter.StopShowingText();
            labelTypewriter.StopDisappearingText();
            labelTypewriter.ShowText(string.Empty);
        }

        await Deactivate();
    }

    private void Update()
    {
        TrackReveal();
    }

    private void LateUpdate()
    {
        if (_follow == null)
        {
            return;
        }

        var camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        transform.position = camera.WorldToScreenPoint(_follow.position + followOffset);
    }
}
