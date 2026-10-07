using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueSequencePlayer : MonoBehaviour, IAdvanceable
{
    [SerializeField] private SimpleDialogueWindow presenter;
    [SerializeField] private DialogueSequence sequence;
    [SerializeField] private bool grabFocus = true;

    private bool _running;
    private bool _advanceRequested;
    private bool _skipRequested;

    public bool IsPlaying => _running;
    public DialogueSequence Sequence => sequence;
    public SimpleDialogueWindow Presenter => presenter;

    public async UniTask Play()
    {
        if (_running || presenter == null || sequence == null)
        {
            return;
        }

        _running = true;
        _advanceRequested = false;
        _skipRequested = false;

        if (grabFocus)
        {
            PlaybackControls.Instance?.Push(this);
        }

        try
        {
            for (var i = 0; i < sequence.Lines.Count; i++)
            {
                if (_skipRequested)
                {
                    break;
                }

                var line = sequence.Lines[i];
                if (line == null)
                {
                    continue;
                }

                await presenter.Show(line);

                var waited = 0f;
                while (!_advanceRequested && !_skipRequested && waited < line.LingerSeconds)
                {
                    waited += Time.deltaTime;
                    await UniTask.Yield();
                }

                _advanceRequested = false;
            }
        }
        finally
        {
            await presenter.Hide();
            _running = false;

            if (grabFocus)
            {
                PlaybackControls.Instance?.Pop(this);
            }
        }
    }

    public void Advance()
    {
        _advanceRequested = true;
    }

    public void SkipAll()
    {
        _skipRequested = true;
        _advanceRequested = true;
    }

    private void OnDestroy()
    {
        if (_running)
        {
            PlaybackControls.Instance?.Pop(this);
        }
    }
}
