#if UNIRX
/// <summary>
/// One thing the player could do right now: the label for the prompt, a gate,
/// and the action itself.
///
/// Interactions are ephemeral. The interactor builds them every frame while a
/// target is focused and throws them away, so nothing should ever subscribe to
/// one. <see cref="Perform"/> calls a method on a persistent object, and it is
/// that object (or the interactor) that exposes observables for others to watch.
/// </summary>
public interface IInteraction
{
    /// <summary>Prompt text, e.g. "Grab chair", "Drop key", "Unlock door".</summary>
    string Text { get; }

    /// <summary>
    /// Re-checked at press time. State can change between hovering and pressing,
    /// so the press never trusts the hover-time answer.
    /// </summary>
    bool CanPerform(IInteractor interactor);

    /// <summary>
    /// Do it. Calls a method on a persistent object; raises that object's
    /// performed/failed observables as appropriate.
    /// </summary>
    void Perform(IInteractor interactor);
}
#endif
