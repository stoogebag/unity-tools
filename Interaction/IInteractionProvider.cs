#if UNIRX
using System.Collections.Generic;

/// <summary>
/// A source of interactions that is not the object being looked at — a carried
/// item, a piece of equipment, a role, or anything else that changes what the
/// player can do.
///
/// Providers register themselves on the interactor (see
/// <see cref="FirstPersonInteractor.RegisterProvider"/>) and are asked to offer
/// interactions every frame while a target is focused.
/// </summary>
public interface IInteractionProvider
{
    /// <summary>
    /// When true, this provider's offers replace everything else: the interactor
    /// stops asking other sources once this provider has offered. Used when
    /// holding something means you cannot do what you would otherwise do, e.g.
    /// carrying a key means you cannot grab a chair.
    /// </summary>
    bool SuppressOtherInteractions { get; }

    /// <summary>
    /// Offer interactions into <paramref name="into"/>. Called every frame while
    /// a target is focused; <paramref name="target"/> may be null when looking at
    /// nothing. Add zero or more entries. Do not clear the list.
    /// </summary>
    void OfferInteractions(IInteractor interactor, Examinable target, List<IInteraction> into);
}
#endif
