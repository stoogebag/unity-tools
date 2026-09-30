using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A first-person ray interactor. Casts forward from an origin, tracks which
/// <see cref="Examinable"/> is under the cursor, and drives focus/examine/interact
/// through the <see cref="IInteractor"/> contract.
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
    private readonly ReactiveProperty<Examinable> _current = new(null);

    /// <summary>What this interactor is currently pointing at, if anything.</summary>
    public IReadOnlyReactiveProperty<Examinable> Current => _current;

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

    private void OnDestroy() => _current.Dispose();

    private void Update()
    {
        var target = Probe();

        if (!ReferenceEquals(target, _current.Value))
        {
            if (_current.Value != null)
                _current.Value.Unfocus(this);

            if (target != null)
                target.Focus(this);

            _current.Value = target;
        }

        if (target == null)
            return;

        if (Press(_interactAction) && target is Interactable interactable)
            interactable.TryInteract(this);

        if (Press(_inspectAction))
            target.TryExamine(this);
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

        var examinable = hit.collider.GetComponentInParent<Examinable>();
        if (examinable == null || !examinable.CanFocus(this))
            return null;

        return examinable;
    }

    public bool HasKey(string key) => _keys.Contains(key);

    public void AddKey(string key) => _keys.Add(key);

    public void RemoveKey(string key) => _keys.Remove(key);

    /// <summary>
    /// Called by the interactable after it raised its own OnInteraction.
    /// Override or subscribe to <see cref="Current"/> for any follow-up the
    /// interactor itself needs to do.
    /// </summary>
    public void Interacted(Interactable interactable)
    {
    }
}
