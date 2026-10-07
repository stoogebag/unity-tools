/// <summary>
/// A playback driver the shared controls can advance and skip: a timeline, a dialogue
/// sequence, or a conversation. Deliberately generic — no Timeline or dialogue types.
/// </summary>
public interface IAdvanceable
{
    void Advance();
    void SkipAll();
}
