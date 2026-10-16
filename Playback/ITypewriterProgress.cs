/// <summary>
/// Reports an in-progress text reveal (a typewriter) so <see cref="PlaybackControls"/> can make
/// advance input two-stage: the first press finishes the reveal, the next advances. Implemented by
/// the dialogue windows; kept in root stoogebag so the generic input owner does not depend on
/// stoogebag.dialogue.
/// </summary>
public interface ITypewriterProgress
{
    bool IsTyping { get; }

    void FinishTyping();

    /// <summary>
    /// True for a short window right after the reveal finished <em>on its own</em>. Advance input in
    /// this window is swallowed: a press meant as "fill the typewriter" that lands just as the line
    /// finishes should not also advance. A fill the player triggered carries no grace.
    /// </summary>
    bool IsInAdvanceGrace { get; }
}
