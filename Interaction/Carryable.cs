#if UNIRX
using UnityEngine;

/// <summary>
/// An item that can be picked up, carried and thrown.
///
/// Pure item: it knows how to be carried, not who is carrying it. The player-side
/// <see cref="ItemCarrier"/> owns the carried state and offers the interactions.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Carryable : Interactable
{
    [SerializeField] private string _displayName = "object";

    private Rigidbody _body;

    public string DisplayName => _displayName;

    private void Awake() => _body = GetComponent<Rigidbody>();

    /// <summary>Called by the carrier when this is picked up.</summary>
    public virtual void OnPickedUp(ItemCarrier carrier)
    {
        if (_body != null)
            _body.isKinematic = true;
    }

    /// <summary>Called by the carrier when this is released.</summary>
    public virtual void OnDropped(ItemCarrier carrier, Vector3 velocity)
    {
        if (_body == null)
            return;

        _body.isKinematic = false;
        _body.linearVelocity = velocity;
    }

    /// <summary>
    /// A carryable declares no interactions of its own — grabbing is offered by
    /// the carrier, which owns the carried state.
    /// </summary>
    public override bool TryInteract(in InteractionContext ctx)
    {
        OnInteractFailed.OnNext(ctx);
        return false;
    }
}
#endif
