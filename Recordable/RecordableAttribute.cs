using UnityEngine;

namespace EditorTools.Recordable
{
    public class RecordableAttribute : PropertyAttribute
    {
        public int MaxLengthSeconds;
        public string SaveFolder;
        public string SaveFolderField;
        public string FileNamePrefix;
        public string FileNameField;

        /// <summary>
        /// Name of a sibling string field to receive the Whisper transcription of the recorded clip.
        /// Leave null to omit the Transcribe control.
        /// </summary>
        public string TranscribeIntoField;

        public RecordableAttribute(
            int maxLengthSeconds = 30,
            string saveFolder = "Resources/audioRecordings",
            string saveFolderField = null,
            string fileNamePrefix = "recording",
            string fileNameField = null,
            string transcribeIntoField = null)
        {
            MaxLengthSeconds = maxLengthSeconds;
            SaveFolder = saveFolder;
            SaveFolderField = saveFolderField;
            FileNamePrefix = fileNamePrefix;
            FileNameField = fileNameField;
            TranscribeIntoField = transcribeIntoField;
        }
    }
}
