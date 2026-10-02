using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// A visual step: watches its upstream sources, plays its effects, and re-emits
/// <see cref="ActivationSource.Settled"/> when they finish. The cap of a button, a
/// lit panel, or a link in a chain.
///
/// It is itself an <see cref="ActivationSource"/>, so later reactions can await it.
/// </summary>
public class ActivationTransition : ActivationSource
{
    [SerializeField] private List<ActivationSource> sources = new();

    private readonly List<ITransitionEffect> _effects = new();
    private readonly CompositeDisposable _subscriptions = new();
    private CancellationTokenSource _cancellation;
    private float _target;

    public void Register(ITransitionEffect effect)
    {
        if (effect == null || _effects.Contains(effect))
        {
            return;
        }

        _effects.Add(effect);
        effect.Snap(_target);
    }

    public void Unregister(ITransitionEffect effect) => _effects.Remove(effect);

    private void OnEnable()
    {
        var streams = sources.Where(s => s != null).Select(s => s.IsActive).ToList();
        if (streams.Count > 0)
        {
            Observable.CombineLatest(streams)
                .Select(all => all.All(a => a))
                .DistinctUntilChanged()
                .Subscribe(_ => Reevaluate())
                .AddTo(_subscriptions);
        }

        Reevaluate();
    }

    private void OnDisable()
    {
        _subscriptions.Clear();
        Cancel();
    }

    private bool AllActive() => sources.Count > 0 && sources.Where(s => s != null).All(s => s.IsActive.Value);

    private void Reevaluate()
    {
        var all = AllActive();
        Active.Value = all;
        PlayTo(all ? 1f : 0f);
    }

    private void PlayTo(float target)
    {
        _target = target;
        Cancel();
        _cancellation = new CancellationTokenSource();
        PlayAsync(target, _cancellation.Token).Forget();
    }

    private async UniTaskVoid PlayAsync(float target, CancellationToken ct)
    {
        var effects = _effects.ToArray();
        if (effects.Length > 0)
        {
            await UniTask.WhenAll(effects.Select(e => e.Play(target, ct)));
        }

        if (!ct.IsCancellationRequested)
        {
            Settled.Value = target >= 1f;
        }
    }

    private void Cancel()
    {
        if (_cancellation == null)
        {
            return;
        }

        _cancellation.Cancel();
        _cancellation.Dispose();
        _cancellation = null;
    }

    protected override void OnDestroy()
    {
        Cancel();
        _subscriptions.Dispose();
        base.OnDestroy();
    }
}
