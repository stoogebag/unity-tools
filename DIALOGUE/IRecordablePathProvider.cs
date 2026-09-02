namespace stoogebag.Dialogue
{
    public interface IRecordablePathProvider
    {
        /// <summary>
        /// Return the full save path relative to Assets, without extension or GUID suffix.
        /// e.g. "timelines/Intro/hello there"
        /// </summary>
        string GetRecordableSavePath(string fieldName);
    }
}
