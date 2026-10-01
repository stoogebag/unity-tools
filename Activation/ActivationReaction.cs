using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// A terminal consequence: watches one or more sources and fires
/// <see cref="OnActivated"/> when they are all active (or settled), otherwise
/// <see cref="OnDeactivated"/>.
/// </summary>
public abstract class ActivationReaction : MonoBehaviour
{
    [SerializeField] private List<ActivationSource> sources = new();
    [SerializeField] private bool waitForSettled;

    private CompositeDisposable _subscriptions;

    private void OnEnable()
    {
        _subscriptions = new CompositeDisposable();

        var streams = sources.Where(s => s != null)
            .Select(s => waitForSettled ? s.IsSettled : s.IsActive)
            .ToList();

        if (streams.Count > 0)
        {
            Observable.CombineLatest(streams)
                .Select(all => all.All(a => a))
                .DistinctUntilChanged()
                .Subscribe(OnChanged)
                .AddTo(_subscriptions);
        }
    }

    private void OnDisable() => _subscriptions?.Dispose();

    private void OnChanged(bool _)
    {
        var active = sources.Where(s => s != null)
            .All(s => waitForSettled ? s.IsSettled.Value : s.IsActive.Value);

        if (active)
        {
            OnActivated();
        }
        else
        {
            OnDeactivated();
        }
    }

    protected abstract void OnActivated();
    protected abstract void OnDeactivated();
}
