using UnityEngine;

/// <summary>Opens / closes a <see cref="Door"/> when its sources go active / inactive.</summary>
public class DoorReaction : ActivationReaction
{
    [SerializeField] private Door door;

    protected override void OnActivated()
    {
        if (door != null)
        {
            door.Open();
        }
    }

    protected override void OnDeactivated()
    {
        if (door != null)
        {
            door.Close();
        }
    }
}
