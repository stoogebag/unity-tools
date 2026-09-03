#if UNITASK && UNIRX
using System.Linq;
using UniRx;
using UnityEngine;

public class PlaySoundOnAction : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private AudioSource _source;

    private GridEntity _ent;

    private void Awake()
    {
        _ent = GetComponent<GridEntity>();
        if (_source == null) _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        var grid = GetComponentInParent<PuzzGrid>();
        if (grid == null) return;

        grid.OnAnimationStartObservable()
            .Subscribe(summary =>
            {
                var moved = summary.ExecutedMoveSummary.Any(a => a.Ent == _ent);
                if (!moved) return;

                var clip = _footstepClips[Random.Range(0, _footstepClips.Length)];
                _source.PlayOneShot(clip);
            })
            .AddTo(this);
    }
}
#endif
