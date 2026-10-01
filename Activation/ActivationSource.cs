using UniRx;
using UnityEngine;

/// <summary>
/// A referencable on/off thing. <see cref="IsActive"/> is the intent (flipped when
/// something is pressed); <see cref="IsSettled"/> is after any visual has finished.
/// A source with no visual mirrors Active onto Settled.
///
/// An abstract class, not an interface: Unity cannot serialize an interface
/// reference, so a reaction's source list could not accept a dragged scene object.
/// </summary>
public class ActivationSource : MonoBehaviour
{
    private readonly ReactiveProperty<bool> _active = new(false);
    private readonly ReactiveProperty<bool> _settled = new(false);

    public IReadOnlyReactiveProperty<bool> IsActive => _active;
    public IReadOnlyReactiveProperty<bool> IsSettled => _settled;

    protected ReactiveProperty<bool> Active => _active;
    protected ReactiveProperty<bool> Settled => _settled;

    /// <summary>Drive the source directly: Active and Settled together.</summary>
    public virtual void SetActive(bool active)
    {
        _active.Value = active;
        _settled.Value = active;
    }

    protected virtual void OnDestroy()
    {
        _active.Dispose();
        _settled.Dispose();
    }
}
