using UnityEngine;

namespace stoogebag.Dialogue
{
    public class RecordableAttribute : PropertyAttribute
    {
        public int MaxLengthSeconds;
        public string SaveFolder;
        public string SaveFolderField;
        public string FileNamePrefix;
        public string FileNameField;

        public RecordableAttribute(
            int maxLengthSeconds = 30,
            string saveFolder = "Resources/audioRecordings",
            string saveFolderField = null,
            string fileNamePrefix = "recording",
            string fileNameField = null)
        {
            MaxLengthSeconds = maxLengthSeconds;
            SaveFolder = saveFolder;
            SaveFolderField = saveFolderField;
            FileNamePrefix = fileNamePrefix;
            FileNameField = fileNameField;
        }
    }
}
