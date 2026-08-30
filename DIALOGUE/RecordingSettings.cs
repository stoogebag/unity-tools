#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Dialogue
{
    public static class RecordingSettings
    {
        private const string Key = "StoogeBag.Recording.MicrophoneDevice";

        public static string SelectedDevice
        {
            get => EditorPrefs.GetString(Key, "");
            set => EditorPrefs.SetString(Key, value);
        }

        public static string[] Devices => Microphone.devices;

        public static string GetActiveDevice()
        {
            var devices = Devices;
            if (devices.Length == 0)
            {
                Debug.LogWarning("No microphone devices found.");
                return null;
            }

            var selected = SelectedDevice;
            if (!string.IsNullOrEmpty(selected) && devices.Contains(selected))
                return selected;

            return devices[0];
        }
    }
}
#endif
