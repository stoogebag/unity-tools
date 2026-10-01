using UnityEngine;

/// <summary>Moves a transform between two authoring poses. Hinge = the markers share a position;
/// slide = they share a rotation.</summary>
public class MoveEffect : TransitionEffect
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;

    protected override void Apply(float progress)
    {
        if (target == null || from == null || to == null)
        {
            return;
        }

        target.localPosition = Vector3.Lerp(from.localPosition, to.localPosition, progress);
        target.localRotation = Quaternion.Slerp(from.localRotation, to.localRotation, progress);
    }
}
