using System.Collections.Generic;
using PurrNet;
using UniRx;
using UnityEngine;

/// <summary>
/// The network half of a carryable. Sits alongside a local <see cref="Carryable"/>
/// and a <see cref="NetworkRigidbody"/> and turns a local grab into an ownership
/// transfer, so the grabber simulates the body and everyone else receives it.
///
/// Grabs are always approved. A new grabber takes the object from the current
/// holder, who is told to drop it. The server only tracks who holds what and
/// grants/removes ownership; the local interaction system is untouched.
///
/// Gravity is toggled through the <see cref="NetworkRigidbody"/> so the setting
/// replicates: a receiver with gravity on would fight the correction spring and
/// only catch up once the object is far away.
/// </summary>
[RequireComponent(typeof(Carryable))]
[RequireComponent(typeof(NetworkRigidbody))]
public class NetworkedCarryable : NetworkBehaviour
{
    private Carryable _carryable;
    private NetworkRigidbody _body;
    private ItemCarrier _carrier;
    private PlayerID? _holder;
    private CompositeDisposable _subscriptions;

    [Header("Contact push")]
    [SerializeField] private float _minContactSpeed = 0.25f;
    [SerializeField] private float _maxContactDeltaV = 2f;
    [SerializeField] private float _maxContactImpulse = 12f;
    [SerializeField] private float _contactCooldown = 0.05f;

    private Rigidbody _rb;
    private readonly Dictionary<int, float> _lastContact = new();

    protected override void OnSpawned()
    {
        base.OnSpawned();

        _carryable = GetComponent<Carryable>();
        _body = GetComponent<NetworkRigidbody>();
        _rb = GetComponent<Rigidbody>();
        _subscriptions = new CompositeDisposable();

        _carryable.PickedUp.Subscribe(OnPickedUp).AddTo(_subscriptions);
        _carryable.Released.Subscribe(OnReleased).AddTo(_subscriptions);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        _subscriptions?.Dispose();
        _subscriptions = null;
    }

    protected override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool asServer)
    {
        base.OnOwnerChanged(oldOwner, newOwner, asServer);

        // The holder keeps the authoritative body and replicates its pose.
        if (_carrier != null)
        {
            if (newOwner == localPlayer)
            {
                _body.useGravity = false;
                _body.ForceSyncFor();
            }

            return;
        }

        // Everyone else publishes the holder's grip, so the box follows the hand
        // instead of waiting for its own pose to replicate.
        if (!newOwner.HasValue)
        {
            _carryable.SetGrip(null);
            return;
        }

        var hold = FindHoldPoint(newOwner.Value);
        if (hold != null)
            _carryable.SetGrip(hold);
    }

    private static Transform FindHoldPoint(PlayerID holder)
    {
        foreach (var carrier in FindObjectsByType<ItemCarrier>(FindObjectsSortMode.None))
        {
            var identity = carrier.GetComponentInParent<NetworkIdentity>();
            if (identity != null && identity.owner == holder)
                return carrier.HoldPoint;
        }

        return null;
    }

    private void OnPickedUp(ItemCarrier carrier)
    {
        _carrier = carrier;
        _body.useGravity = false;

        if (isServer)
            Grant(localPlayer ?? default);
        else
            RequestGrabServerRpc();
    }

    private void OnReleased(ItemCarrier carrier)
    {
        _carrier = null;
        _body.useGravity = true;

        if (isServer)
            Release(localPlayer ?? default);
        else
            RequestReleaseServerRpc();
    }

    [ServerRpc]
    private void RequestGrabServerRpc(RPCInfo info = default) => Grant(info.sender);

    [ServerRpc]
    private void RequestReleaseServerRpc(RPCInfo info = default) => Release(info.sender);

    private void Grant(PlayerID grabber)
    {
        var previous = _holder;
        _holder = grabber;

        if (previous.HasValue && previous.Value != grabber)
            ForceDropRpc(previous.Value);

        GiveOwnership(grabber);
    }

    private void Release(PlayerID releaser)
    {
        if (_holder != releaser)
            return;

        _holder = null;
        RemoveOwnership();
    }

    [TargetRpc]
    private void ForceDropRpc(PlayerID player)
    {
        _carrier?.Drop();
        _carrier = null;
    }

    private void OnCollisionEnter(Collision collision) => ReportContact(collision);

    private void OnCollisionStay(Collision collision) => ReportContact(collision);

    private void ReportContact(Collision collision)
    {
        if (!isSpawned || !isOwner || collision.collider == null || _rb == null)
            return;

        var otherBody = collision.collider.attachedRigidbody;
        if (otherBody == null)
            return;

        var target = otherBody.GetComponent<NetworkRigidbody>();
        if (target == null || target == _body || target.owner.HasValue)
            return;

        var toOther = otherBody.position - _rb.position;
        if (toOther.sqrMagnitude < 1e-6f)
            return;

        var direction = toOther.normalized;
        var closing = Vector3.Dot(_rb.linearVelocity - otherBody.linearVelocity, direction);
        if (closing < _minContactSpeed)
            return;

        // Scale the impulse with the target's mass so it reaches the capped
        // closing speed, then cap the total so a heavy body is not launched.
        var deltaV = Mathf.Min(closing, _maxContactDeltaV);
        var impulse = direction * (deltaV * otherBody.mass);
        if (impulse.sqrMagnitude > _maxContactImpulse * _maxContactImpulse)
            impulse = impulse.normalized * _maxContactImpulse;

        var key = collision.collider.GetInstanceID();
        var now = Time.time;
        if (_lastContact.TryGetValue(key, out var last) && now - last < _contactCooldown)
            return;
        _lastContact[key] = now;

        var point = collision.contactCount > 0 ? collision.GetContact(0).point : otherBody.position;
        ReportContactServerRpc(target, impulse, point);
    }

    [ServerRpc]
    private void ReportContactServerRpc(NetworkRigidbody other, Vector3 impulse, Vector3 point)
    {
        if (other == null || other.owner.HasValue)
            return;

        if (impulse.sqrMagnitude > _maxContactImpulse * _maxContactImpulse)
            impulse = impulse.normalized * _maxContactImpulse;

        other.AddForceAtPosition(impulse, point, ForceMode.Impulse);
    }
}
