using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// One cosmetic animation with a duration, driven by progress rather than a tween:
/// the effect owns a 0..1 value and renders it, so mid-flight cancellation and
/// reversal can never wedge it.
/// </summary>
public interface ITransitionEffect
{
    /// <summary>Animate progress toward <paramref name="target"/> (0 or 1). Completes when done or cancelled.</summary>
    UniTask Play(float target, CancellationToken ct);

    /// <summary>Set the visual to progress without animating.</summary>
    void Snap(float target);
}

/// <summary>
/// Base for effects. Effects register themselves with their nearest ancestor
/// <see cref="ActivationTransition"/> — the dependency points up.
/// </summary>
public abstract class TransitionEffect : MonoBehaviour, ITransitionEffect
{
    [SerializeField] protected float Duration = 0.3f;
    [SerializeField] protected AnimationCurve Ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    protected float Progress { get; private set; }

    protected abstract void Apply(float progress);

    protected virtual void OnEnable()
    {
        Transition?.Register(this);
    }

    protected virtual void OnDisable()
    {
        Transition?.Unregister(this);
    }

    private ActivationTransition Transition => GetComponentInParent<ActivationTransition>();

    public async UniTask Play(float target, CancellationToken ct)
    {
        var start = Progress;
        var elapsed = 0f;

        if (Duration > 0f)
        {
            while (elapsed < Duration)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                elapsed += Time.deltaTime;
                Progress = Mathf.Lerp(start, target, Ease.Evaluate(Mathf.Clamp01(elapsed / Duration)));
                Apply(Progress);
                await UniTask.Yield();
            }
        }

        if (ct.IsCancellationRequested)
        {
            return;
        }

        Progress = target;
        Apply(Progress);
    }

    public void Snap(float target)
    {
        Progress = target;
        Apply(Progress);
    }
}
