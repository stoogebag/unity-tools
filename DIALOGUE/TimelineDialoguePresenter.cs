using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Bridges dialogue raised by a Timeline (<see cref="DialogueTimelineBehaviour"/>) to a
/// <see cref="SimpleDialogueWindow"/>, so cutscene lines present as subtitles. The window is shown
/// on each line trigger and hidden when the line's clip pauses or the timeline stops. When a
/// <see cref="director"/> is set the presenter only handles that timeline's dialogue — the trigger
/// events are static, so a scene with more than one dialogue timeline needs this to avoid cross-talk.
/// </summary>
public class TimelineDialoguePresenter : MonoBehaviour
{
    [SerializeField] private SimpleDialogueWindow window;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool hideOnEnd = true;

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private void OnEnable()
    {
        DialogueTimelineBehaviour.DialogueTriggeredObservable.Subscribe(OnTriggered).AddTo(_disposables);
        DialogueTimelineBehaviour.DialogueEndedObservable.Subscribe(OnEnded).AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables.Clear();
    }

    private bool Owns(DialogueTimelineBehaviour behaviour)
    {
        return behaviour != null && (director == null || behaviour.director == director);
    }

    private void OnTriggered(DialogueTimelineBehaviour behaviour)
    {
        if (window == null || !Owns(behaviour))
        {
            return;
        }

        window.Show(behaviour.Line).Forget();
    }

    private void OnEnded(DialogueTimelineBehaviour behaviour)
    {
        if (window == null || !hideOnEnd || !Owns(behaviour))
        {
            return;
        }

        window.Hide().Forget();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
