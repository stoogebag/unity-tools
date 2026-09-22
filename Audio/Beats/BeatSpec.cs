using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    /// <summary>
    /// Parses the shared beats CSV string (e.g. "1,3,4" or "1, 1.5").
    /// CSV only: no ranges, no wildcards. Fractional beats allowed.
    /// </summary>
    public static class BeatSpec
    {
        public static bool TryParse(string input, out List<float> beats, out string error)
        {
            beats = new List<float>();
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Beats string is empty. Use comma-separated beat numbers, e.g. \"1,3,4\".";
                return false;
            }

            var seen = new HashSet<float>();
            var tokens = input.Split(',');

            foreach (var raw in tokens)
            {
                var token = raw.Trim();
                if (string.IsNullOrEmpty(token))
                    continue;

                if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out float beat))
                {
                    error = $"Could not parse beat '{token}'. Use comma-separated numbers, e.g. \"1,3,4\".";
                    continue;
                }

                if (beat < 1f)
                {
                    error = $"Beat '{token}' is below 1. Beat numbers start at 1.";
                    continue;
                }

                if (seen.Add(beat))
                    beats.Add(beat);
            }

            if (beats.Count == 0 && error == null)
                error = "No valid beats found. Use comma-separated beat numbers, e.g. \"1,3,4\".";

            beats.Sort();
            return beats.Count > 0;
        }
    }
}
