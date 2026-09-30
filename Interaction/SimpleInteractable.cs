#if UNIRX
#if CINEMACHINE
public class SimpleInteractable : Interactable
{
    public override bool TryInteract(in InteractionContext ctx)
    {
        OnInteractPerformed.OnNext(ctx);
        return true;
    }
}
#endif
#endif
