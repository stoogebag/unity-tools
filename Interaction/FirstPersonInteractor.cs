using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A first-person ray interactor. Casts forward from an origin, tracks which
/// <see cref="Examinable"/> is under the cursor, resolves what the interact key
/// would do right now, and sends pressing.
///
/// Local only: focus, examine and prompts are per-player. Only the effect of an
/// interaction may need networking, and that is the interactable's concern.
/// </summary>
public class FirstPersonInteractor : MonoBehaviour, IInteractor
{
    [Header("Ray")]
    [SerializeField] private Transform _origin;
    [SerializeField] private float _range = 5f;
    [SerializeField] private LayerMask _mask = ~0;

    [Tooltip("Layers to skip when looking for a target. Use this to exclude the " +
             "interactor's own body; other players can remain valid targets.")]
    [SerializeField] private LayerMask _ignore;

    [Header("Input")]
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private InputActionReference _inspectAction;

    private readonly HashSet<string> _keys = new();
    private readonly List<IInteractionProvider> _providers = new();
    private readonly List<IInteraction> _buffer = new();

    private readonly ReactiveProperty<Examinable> _target = new(null);
    private readonly ReactiveProperty<IInteraction> _current = new(null);

    /// <summary>What this interactor is currently pointing at, if anything.</summary>
    public IReadOnlyReactiveProperty<Examinable> Target => _target;

    /// <summary>What pressing interact would do right now, if anything.</summary>
    public IReadOnlyReactiveProperty<IInteraction> Current => _current;

    /// <summary>
    /// Fired just before an interaction is performed. The interaction text is
    /// still the one the player saw (it can change once the effect lands, e.g. a
    /// door's "Open" becoming "Close"), so watch this for what the player chose.
    /// </summary>
    public event Action<IInteraction> OnPerforming;
    public IObservable<IInteraction> OnPerformingObservable =>
        Observable.FromEvent<IInteraction>(h => OnPerforming += h, h => OnPerforming -= h);

    /// <summary>
    /// Fired after an interaction is performed. The interaction itself is
    /// ephemeral, so watch this (or the target's own observables) rather than
    /// subscribing to an <see cref="IInteraction"/>.
    /// </summary>
    public event Action<IInteraction> OnPerformed;
    public IObservable<IInteraction> OnPerformedObservable =>
        Observable.FromEvent<IInteraction>(h => OnPerformed += h, h => OnPerformed -= h);

    /// <summary>
    /// Fired when the player examines a target. Examine is its own channel,
    /// separate from interactions — it is not offered and cannot be suppressed.
    /// </summary>
    public event Action<Examinable> OnExamined;
    public IObservable<Examinable> OnExaminedObservable =>
        Observable.FromEvent<Examinable>(h => OnExamined += h, h => OnExamined -= h);

    private void Awake()
    {
        if (_origin == null)
            _origin = transform;
    }

    private void OnEnable()
    {
        _interactAction?.action.Enable();
        _inspectAction?.action.Enable();
    }

    private void OnDisable()
    {
        _interactAction?.action.Disable();
        _inspectAction?.action.Disable();
    }

    private void OnDestroy()
    {
        _target.Dispose();
        _current.Dispose();
    }

    public void RegisterProvider(IInteractionProvider provider)
    {
        if (provider != null && !_providers.Contains(provider))
            _providers.Add(provider);
    }

    public void UnregisterProvider(IInteractionProvider provider)
    {
        _providers.Remove(provider);
    }

    private void Update()
    {
        var target = Probe();
        UpdateTarget(target);
        UpdateCurrent(target);

        if (Press(_inspectAction) && target != null)
        {
            target.TryExamine(this);
            OnExamined?.Invoke(target);
        }

        if (Press(_interactAction))
            PerformCurrent();
    }

    private void UpdateTarget(Examinable target)
    {
        if (ReferenceEquals(target, _target.Value))
            return;

        if (_target.Value != null)
            _target.Value.Unfocus(this);

        if (target != null)
            target.Focus(this);

        _target.Value = target;
    }

    private void UpdateCurrent(Examinable target)
    {
        _buffer.Clear();

        // Providers first. A suppressing provider means we stop looking: its
        // offers replace everything else, including the target's own.
        var suppressed = false;
        for (var i = 0; i < _providers.Count; i++)
        {
            var provider = _providers[i];
            provider.OfferInteractions(this, target, _buffer);

            if (provider.SuppressOtherInteractions)
            {
                suppressed = true;
                break;
            }
        }

        if (!suppressed && target is Interactable interactable)
            interactable.OfferInteractions(this, _buffer);

        _current.Value = Resolve();
    }

    private IInteraction Resolve()
    {
        for (var i = 0; i < _buffer.Count; i++)
            if (_buffer[i].CanPerform(this))
                return _buffer[i];

        return null;
    }

    private bool PerformCurrent()
    {
        var interaction = _current.Value;
        if (interaction == null || !interaction.CanPerform(this))
            return false;

        OnPerforming?.Invoke(interaction);
        interaction.Perform(this);
        OnPerformed?.Invoke(interaction);

        // tell the focused object too, so observers can watch a specific object
        // rather than everything the player does
        if (_target.Value is Interactable interactable)
            interactable.OnInteractPerformed.OnNext(new InteractionContext(this, interactable));

        return true;
    }

    private static bool Press(InputActionReference reference)
    {
        return reference != null && reference.action != null && reference.action.WasPressedThisFrame();
    }

    private Examinable Probe()
    {
        if (_origin == null)
            return null;

        var mask = _mask & ~_ignore;
        if (!Physics.Raycast(_origin.position, _origin.forward, out var hit, _range, mask, QueryTriggerInteraction.Collide))
            return null;

        return hit.collider.GetComponentInParent<Examinable>();
    }

    public bool HasKey(string key) => _keys.Contains(key);

    public void AddKey(string key) => _keys.Add(key);

    public void RemoveKey(string key) => _keys.Remove(key);
}
