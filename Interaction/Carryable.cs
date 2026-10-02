#if UNIRX
using UnityEngine;

/// <summary>
/// An item that can be picked up, carried and thrown.
///
/// Pure item: it knows how to be carried, not who is carrying it. The player-side
/// <see cref="ItemCarrier"/> owns the carried state and offers the interactions.
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
    private Transform _holdPoint;
    private bool _carried;
    private CollisionDetectionMode _restingCollisionMode;

    public string DisplayName => _displayName;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    /// <summary>Called by the carrier when this is picked up.</summary>
    public virtual void OnPickedUp(ItemCarrier carrier, Transform holdPoint)
    {
        _holdPoint = holdPoint;
        _carried = true;

        if (_body == null)
            return;

        _restingCollisionMode = _body.collisionDetectionMode;
        _body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _body.useGravity = false;
        _body.isKinematic = false;
        _body.WakeUp();
    }

    /// <summary>Called by the carrier when this is released.</summary>
    public virtual void OnDropped(ItemCarrier carrier, Vector3 velocity)
    {
        _carried = false;
        _holdPoint = null;

        if (_body == null)
            return;

        _body.collisionDetectionMode = _restingCollisionMode;
        _body.useGravity = true;
        _body.isKinematic = false;
        _body.linearVelocity = velocity;
    }

    private void FixedUpdate()
    {
        if (!_carried || _body == null || _holdPoint == null)
            return;

        var dt = Time.fixedDeltaTime;

        _body.linearVelocity += (_holdPoint.position - _body.position) * _spring * dt;
        _body.linearVelocity *= 1f / (1f + _damping * dt);

        var delta = _holdPoint.rotation * Quaternion.Inverse(_body.rotation);
        delta.ToAngleAxis(out var angle, out var axis);
        if (angle > 180f)
            angle -= 360f;

        _body.angularVelocity += axis * (angle * Mathf.Deg2Rad) * _angularSpring * dt;
        _body.angularVelocity *= 1f / (1f + _angularDamping * dt);
    }
}
#endif
