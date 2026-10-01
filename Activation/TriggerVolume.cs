using UnityEngine;

/// <summary>
/// A physical trigger that drives an <see cref="ActivationSource"/>. Put a trigger
/// collider on the same object. With <see cref="latch"/> it stays active once
/// entered (an encounter that fires once); without it, it mirrors presence.
/// </summary>
public class TriggerVolume : ActivationSource
{
    [SerializeField] private LayerMask mask = ~0;
    [SerializeField] private bool latch = true;

    private void OnTriggerEnter(Collider other)
    {
        if (InMask(other))
        {
            SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!latch && InMask(other))
        {
            SetActive(false);
        }
    }

    private bool InMask(Collider other) => (mask.value & (1 << other.gameObject.layer)) != 0;
}
