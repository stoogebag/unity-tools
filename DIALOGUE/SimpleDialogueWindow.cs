using Cysharp.Threading.Tasks;
using Febucci.TextAnimatorForUnity;
using stoogebag.Extensions;
using stoogebag.UITools.Windows;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDialogueWindow : Window
{
    [SerializeField] private TypewriterComponent textTypewriter;
    [SerializeField] private TypewriterComponent labelTypewriter;
    [SerializeField] private Image portrait;
    [SerializeField] private Window nextIndicator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool followSpeaker;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 0.5f, 0f);

    private Transform _follow;

    public async UniTask Show(DialogueLine line)
    {
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

        await textTypewriter.ShowTextAndAwait(line.Text ?? string.Empty);

        if (nextIndicator != null)
        {
            nextIndicator.Activate().Forget();
        }
    }

    public async UniTask Hide()
    {
        _follow = null;

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
