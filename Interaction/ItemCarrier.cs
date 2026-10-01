#if UNIRX
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// The player-side half of carrying. Owns what is currently carried and offers
/// the interactions for it.
///
/// Lives on its own child of the interactor and registers with it as an
/// <see cref="IInteractionProvider"/>, so its offers take precedence: while
/// carrying, nothing else is offered.
/// </summary>
public class ItemCarrier : MonoBehaviour, IInteractionProvider
{
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _throwPower = 6f;

    private FirstPersonInteractor _interactor;
    private readonly ReactiveProperty<Carryable> _carried = new(null);

    // caches so a given interaction instance is reused rather than rebuilt each
    // frame; ReactiveProperty dedupes on reference, so stability matters
    private readonly Dictionary<Carryable, GrabInteraction> _grabCache = new();
    private DropInteraction _drop;

    /// <summary>What is being carried right now, if anything.</summary>
    public IReadOnlyReactiveProperty<Carryable> Carried => _carried;

    public bool IsCarrying => _carried.Value != null;

    private void Awake()
    {
        _interactor = GetComponentInParent<FirstPersonInteractor>();
        if (_interactor == null)
            Debug.LogError("ItemCarrier: no FirstPersonInteractor found in parents", this);

        if (_holdPoint == null)
            _holdPoint = transform;

        _drop = new DropInteraction(this);
    }

    private void OnEnable()
    {
        if (_interactor != null)
            _interactor.RegisterProvider(this);
    }

    private void OnDisable()
    {
        if (_interactor != null)
            _interactor.UnregisterProvider(this);
    }

    private void OnDestroy() => _carried.Dispose();

    public void PickUp(Carryable item)
    {
        if (_carried.Value != null || item == null)
            return;

        _carried.Value = item;
        item.OnPickedUp(this, _holdPoint);
    }

    public void Drop() => Release(Vector3.zero);

    public void Throw()
    {
        var forward = _interactor != null ? _interactor.transform.forward : transform.forward;
        Release(forward * _throwPower);
    }

    private void Release(Vector3 velocity)
    {
        var item = _carried.Value;
        if (item == null)
            return;

        _carried.Value = null;
        item.OnDropped(this, velocity);
    }

    // --- IInteractionProvider ---------------------------------------------

    public bool SuppressOtherInteractions => IsCarrying;

    public void OfferInteractions(IInteractor interactor, Examinable target, List<IInteraction> into)
    {
        if (IsCarrying)
        {
            into.Add(_drop);
            return;
        }

        if (target is Carryable item)
        {
            if (!_grabCache.TryGetValue(item, out var grab))
            {
                grab = new GrabInteraction(item, this);
                _grabCache[item] = grab;
            }

            into.Add(grab);
        }
    }
}
#endif
