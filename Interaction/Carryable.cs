#if UNIRX
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// An item that can be picked up, carried and thrown.
///
/// Pure item: it knows how to be carried, not who is carrying it. The player-side
/// <see cref="ItemCarrier"/> owns the carried state and offers the interactions.
///
/// The grip is a single observable: non-null while carried, pointing at the hold
/// transform. The local grab and the network layer both publish into it, and the
/// body consequences (gravity, collision mode, the pull spring) are listeners on
/// it. A peer that never gets a grip simply never reacts.
///
/// The body stays dynamic and in the world while carried. A velocity spring pulls
/// it to the hold point each fixed step, so it trails and wobbles instead of being
/// welded in place.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Carryable : Interactable
{
    [SerializeField] private string _displayName = "object";
    [SerializeField] private float _spring = 120f;
    [SerializeField] private float _damping = 8f;
    [SerializeField] private float _angularSpring = 60f;
    [SerializeField] private float _angularDamping = 8f;

    private Rigidbody _body;
    private bool _gripping;
    private CollisionDetectionMode _restingCollisionMode;

    private readonly ReactiveProperty<Transform> _grip = new(null);
    private readonly Subject<ItemCarrier> _pickedUp = new();
    private readonly Subject<ItemCarrier> _released = new();

    public string DisplayName => _displayName;

    /// <summary>The hold transform while carried, null otherwise.</summary>
    public IReadOnlyReactiveProperty<Transform> Grip => _grip;

    /// <summary>Fires on the peer that picked this up, with its carrier.</summary>
    public IObservable<ItemCarrier> PickedUp => _pickedUp;

    /// <summary>Fires on the peer that released this, with its last carrier.</summary>
    public IObservable<ItemCarrier> Released => _released;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _grip.Subscribe(OnGripChanged).AddTo(this);
    }

    private void OnDestroy()
    {
        _pickedUp.Dispose();
        _released.Dispose();
        _grip.Dispose();
    }

    /// <summary>Called by the carrier when this is picked up.</summary>
    public virtual void OnPickedUp(ItemCarrier carrier, Transform holdPoint)
    {
        SetGrip(holdPoint);
        _pickedUp.OnNext(carrier);
    }

    /// <summary>Called by the carrier when this is released.</summary>
    public virtual void OnDropped(ItemCarrier carrier, Vector3 velocity)
    {
        if (_body != null)
            _body.linearVelocity = velocity;

        SetGrip(null);
        _released.OnNext(carrier);
    }

    /// <summary>
    /// Points the carry at a hold transform, or clears it with null. The local
    /// interaction and the network layer both publish through here.
    /// </summary>
    public void SetGrip(Transform holdPoint) => _grip.Value = holdPoint;

    private void OnGripChanged(Transform holdPoint)
    {
        if (_body == null)
            return;

        bool gripping = holdPoint != null;
        if (gripping == _gripping)
            return;

        _gripping = gripping;

        if (gripping)
        {
            _restingCollisionMode = _body.collisionDetectionMode;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _body.useGravity = false;
            _body.isKinematic = false;
            _body.WakeUp();
        }
        else
        {
            _body.collisionDetectionMode = _restingCollisionMode;
            _body.useGravity = true;
            _body.isKinematic = false;
        }
    }

    private void FixedUpdate()
    {
        var holdPoint = _grip.Value;
        if (holdPoint == null || _body == null)
            return;

        var dt = Time.fixedDeltaTime;

        _body.linearVelocity += (holdPoint.position - _body.position) * _spring * dt;
        _body.linearVelocity *= 1f / (1f + _damping * dt);

        var delta = holdPoint.rotation * Quaternion.Inverse(_body.rotation);
        delta.ToAngleAxis(out var angle, out var axis);
        if (angle > 180f)
            angle -= 360f;

        _body.angularVelocity += axis * (angle * Mathf.Deg2Rad) * _angularSpring * dt;
        _body.angularVelocity *= 1f / (1f + _angularDamping * dt);

        _body.WakeUp();
    }
}
#endif
