using UnityEngine;

/// <summary>
/// A physical trigger that drives an <see cref="ActivationSource"/>. Put a trigger
/// collider on the same object. <see cref="IsActive"/> mirrors presence; the counts
/// expose history and occupancy so consumers can distinguish "inside now" from
/// "has been inside at least once".
/// </summary>
public class TriggerVolume : ActivationSource
{
    [SerializeField] private LayerMask mask = ~0;

    public int TriggerCount { get; private set; }
    public int Occupants { get; private set; }

    public bool HasBeenTriggered => TriggerCount > 0;
    public bool Inside => Occupants > 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!InMask(other))
            return;

        TriggerCount++;
        Occupants++;
        SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!InMask(other))
            return;

        Occupants--;
        if (Occupants <= 0)
        {
            Occupants = 0;
            SetActive(false);
        }
    }

    private bool InMask(Collider other) => (mask.value & (1 << other.gameObject.layer)) != 0;
}
