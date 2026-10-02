#if UNITY_EDITOR
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using stoogebag.Utils;
using UnityEditor;
using UnityEngine;
#if WHISPER
using Whisper;
#endif

namespace EditorTools.Recordable.Editor
{
    /// <summary>
    /// The single implementation of editor microphone recording, WAV saving, preview playback
    /// and optional Whisper transcription. Used by <see cref="RecordablePropertyDrawer"/> and by
    /// any custom editor UI (e.g. the VIDE graph editor) that cannot use a property drawer.
    /// </summary>
    public static class RecordableAudio
    {
        public static string GetActiveDevice() => RecordingSettings.GetActiveDevice();

        public static AudioClip Start(string device, int maxLengthSeconds)
        {
            if (string.IsNullOrEmpty(device))
            {
                Debug.LogWarning("[Recordable] No microphone device available.");
                return null;
            }

            return Microphone.Start(device, false, maxLengthSeconds, 44100);
        }

        public static void Stop(string device)
        {
            if (!string.IsNullOrEmpty(device))
                Microphone.End(device);
        }

        /// <summary>
        /// Trims, writes the clip as a WAV under Assets/, imports it, and returns the imported clip.
        /// </summary>
        public static AudioClip Save(AudioClip tempClip, string basePath)
        {
            if (tempClip == null) return null;

            var wavPath = $"{basePath}-{Guid.NewGuid()}";

            var trimmed = SavWav.TrimSilence(tempClip, 0.001f);
            SavWav.Save(wavPath, trimmed);

            Thread.Sleep(10);
            AssetDatabase.ImportAsset("Assets/" + wavPath + ".wav", ImportAssetOptions.ForceSynchronousImport);
            Thread.Sleep(10);

            return AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/" + wavPath + ".wav");
        }

        public static void Play(AudioClip clip)
        {
            if (clip != null)
                AudioUtils.PlayPreviewClip(clip);
        }

        /// <summary>
        /// Transcribes the clip with Whisper. Returns null (and logs) when Whisper is unavailable.
        /// </summary>
        public static async UniTask<string> Transcribe(AudioClip clip)
        {
#if WHISPER
            if (clip == null) return null;

            var manager = GameObjectExtensions.FindObjectOfTypeInActiveScene<WhisperManager>();
            if (manager == null)
            {
                Debug.LogWarning("[Recordable] No WhisperManager in the active scene.");
                return null;
            }

            await manager.InitModel();
            var res = await manager.GetTextAsync(clip);
            return res?.Result?.Trim(' ');
#else
            Debug.Log("[Recordable] Whisper is not installed.");
            await UniTask.CompletedTask;
            return null;
#endif
        }
    }
}
#endif
