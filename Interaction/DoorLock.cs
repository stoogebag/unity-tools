using UnityEngine;

/// <summary>Bars a <see cref="Door"/> while its sources are active.</summary>
public class DoorLock : ActivationReaction
{
    [SerializeField] private Door door;
    [SerializeField] private bool forceClosed = true;

    private void Awake()
    {
        if (door == null)
        {
            door = GetComponentInParent<Door>();
        }
    }

    protected override void OnActivated()
    {
        if (door == null)
        {
            return;
        }

        if (forceClosed)
        {
            door.Close();
        }

        door.SetLocked(true);
    }

    protected override void OnDeactivated()
    {
        if (door != null)
        {
            door.SetLocked(false);
        }
    }
}
