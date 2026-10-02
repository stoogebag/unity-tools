#if UNIRX
#if CINEMACHINE
public readonly struct InteractionContext
{
    public readonly IInteractor Interactor;
    public readonly Interactable Target;

    public InteractionContext(IInteractor interactor, Interactable target)
    {
        Interactor = interactor;
        Target = target;
    }
}
#endif
#endif
