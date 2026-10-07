using UnityEngine.Playables;

/// <summary>
/// Marks a timeline clip as a hold point: the timeline stops there and routes advance input
/// to OnAdvance. Timeline-internal in spirit — a timeline is a bag of clips and must find the
/// one currently holding; linear drivers (sequences, conversations) implement IAdvanceable
/// directly instead. Lives in root stoogebag because DialogueClip (stoogebag.dialogue) must
/// implement it, and stoogebag.dialogue cannot reference stoogebag.Timeline.
/// </summary>
public interface IHoldPoint
{
    /// <summary>Per-clip opt-out: whether the timeline should actually stop on this clip.</summary>
    bool WantsHold { get; }

    void OnAdvance(PlayableDirector director);
}
