#if UNIRX
/// <summary>Pick up the target item.</summary>
public sealed class GrabInteraction : IInteraction
{
    private readonly Carryable _item;
    private readonly ItemCarrier _carrier;

    public GrabInteraction(Carryable item, ItemCarrier carrier)
    {
        _item = item;
        _carrier = carrier;
    }

    public string Text => $"Grab {_item.DisplayName}";

    public bool CanPerform(IInteractor interactor) => _item != null && _carrier != null && !_carrier.IsCarrying;

    public void Perform(IInteractor interactor) => _carrier.PickUp(_item);
}

/// <summary>Put down what is currently carried.</summary>
public sealed class DropInteraction : IInteraction
{
    private readonly ItemCarrier _carrier;

    public DropInteraction(ItemCarrier carrier) => _carrier = carrier;

    public string Text
    {
        get
        {
            var carried = _carrier != null ? _carrier.Carried.Value : null;
            return carried != null ? $"Drop {carried.DisplayName}" : "Drop";
        }
    }

    public bool CanPerform(IInteractor interactor) => _carrier != null && _carrier.IsCarrying;

    public void Perform(IInteractor interactor) => _carrier.Drop();
}
#endif
