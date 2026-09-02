#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace stoogebag.Utils
{
    public static class AudioUtils
    {
        private static MethodInfo _playPreviewClipMethod;

        public static void PlayPreviewClip(AudioClip clip)
        {
            if (clip == null) return;

            var method = GetPlayPreviewClipMethod();
            if (method != null)
            {
                method.Invoke(null, new object[] { clip, 0, false });
            }
            else
            {
                Debug.LogWarning("[AudioUtils] Could not find UnityEditor.AudioUtil.PlayPreviewClip.");
            }
        }

        private static MethodInfo GetPlayPreviewClipMethod()
        {
            if (_playPreviewClipMethod == null)
            {
                var audioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
                if (audioUtilType != null)
                {
                    _playPreviewClipMethod = audioUtilType.GetMethod(
                        "PlayPreviewClip",
                        BindingFlags.Static | BindingFlags.Public,
                        null,
                        new[] { typeof(AudioClip), typeof(int), typeof(bool) },
                        null);
                }
            }

            return _playPreviewClipMethod;
        }
    }
}
#endif
