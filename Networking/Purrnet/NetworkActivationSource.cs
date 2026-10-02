using PurrNet;
using UniRx;
using UnityEngine;

/// <summary>
/// The network half of an <see cref="ActivationSource"/>. The source stays the
/// local, observable on/off state; this component makes it authoritative.
///
/// A local change to the source is treated as an intent: on the server it is
/// written to the synced value, on a client it is sent to the server. The synced
/// value is authoritative and is applied back to the local source on every peer,
/// including late joiners.
/// </summary>
[RequireComponent(typeof(ActivationSource))]
public class NetworkActivationSource : NetworkBehaviour
{
    [SerializeField] private SyncVar<bool> _active = new SyncVar<bool>(false);

    private ActivationSource _source;
    private CompositeDisposable _subscriptions;
    private bool _applying;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        _source = GetComponent<ActivationSource>();
        _subscriptions = new CompositeDisposable();
        _source.IsActive.Subscribe(OnLocalChanged).AddTo(_subscriptions);
        _active.onChanged += Apply;

        if (isServer)
            _active.value = _source.IsActive.Value;

        ApplyLocal(_active.value);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        _active.onChanged -= Apply;
        _subscriptions?.Dispose();
        _subscriptions = null;
    }

    private void OnLocalChanged(bool value)
    {
        if (_applying || _active.value == value)
            return;

        if (isServer)
            _active.value = value;
        else
            RequestSetServerRpc(value);
    }

    private void Apply(bool value) => ApplyLocal(value);

    private void ApplyLocal(bool value)
    {
        _applying = true;
        _source.SetActive(value);
        _applying = false;
    }

    [ServerRpc]
    private void RequestSetServerRpc(bool value, RPCInfo info = default) => _active.value = value;
}
