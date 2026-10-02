using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets GameObjects active/inactive when its sources activate. One-shot: deactivation
/// does nothing (a reaction on a death source must not undo itself at enable).
/// </summary>
public class SetActiveReaction : ActivationReaction
{
    [SerializeField] private List<GameObject> targets = new();
    [SerializeField] private bool active = false;

    protected override void OnActivated()
    {
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }

    protected override void OnDeactivated()
    {
    }
}
