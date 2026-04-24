using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class AudioSourcePitch
    {
        //this is the offset that brings the original clip into the desired key.
        public float TonicMultiplier = 1f;
    }

    public static class PitchMultipliers
    {
        public const float SemitoneMultiplier = 1.059463f;
        private const float Octave2Factor = 2f;
        private const float Octave3Factor = 4f;

        // Octave 1
        public const float A1 = 1f;
        public const float ASharp1 = SemitoneMultiplier;
        public const float B1 = SemitoneMultiplier * ASharp1;
        public const float C1 = SemitoneMultiplier * B1;
        public const float CSharp1 = SemitoneMultiplier * C1;
        public const float D1 = SemitoneMultiplier * CSharp1;
        public const float DSharp1 = SemitoneMultiplier * D1;
        public const float E1 = SemitoneMultiplier * DSharp1;
        public const float F1 = SemitoneMultiplier * E1;
        public const float FSharp1 = SemitoneMultiplier * F1;
        public const float G1 = SemitoneMultiplier * FSharp1;
        public const float GSharp1 = SemitoneMultiplier * G1;

        // Octave 2
        public const float A2 = A1 * Octave2Factor;
        public const float ASharp2 = ASharp1 * Octave2Factor;
        public const float B2 = B1 * Octave2Factor;
        public const float C2 = C1 * Octave2Factor;
        public const float CSharp2 = CSharp1 * Octave2Factor;
        public const float D2 = D1 * Octave2Factor;
        public const float DSharp2 = DSharp1 * Octave2Factor;
        public const float E2 = E1 * Octave2Factor;
        public const float F2 = F1 * Octave2Factor;
        public const float FSharp2 = FSharp1 * Octave2Factor;
        public const float G2 = G1 * Octave2Factor;
        public const float GSharp2 = GSharp1 * Octave2Factor;

        // Octave 3
        public const float A3 = A1 * Octave3Factor;
        public const float ASharp3 = ASharp1 * Octave3Factor;
        public const float B3 = B1 * Octave3Factor;
        public const float C3 = C1 * Octave3Factor;
        public const float CSharp3 = CSharp1 * Octave3Factor;
        public const float D3 = D1 * Octave3Factor;
        public const float DSharp3 = DSharp1 * Octave3Factor;
        public const float E3 = E1 * Octave3Factor;
        public const float F3 = F1 * Octave3Factor;
        public const float FSharp3 = FSharp1 * Octave3Factor;
        public const float G3 = G1 * Octave3Factor;
        public const float GSharp3 = GSharp1 * Octave3Factor;

        // Flats (Octave 1)
        public const float BFlat1 = ASharp1;
        public const float DFlat1 = CSharp1;
        public const float EFlat1 = DSharp1;
        public const float GFlat1 = FSharp1;
        public const float AFlat1 = GSharp1;

        // Flats (Octave 2)
        public const float BFlat2 = ASharp2;
        public const float DFlat2 = CSharp2;
        public const float EFlat2 = DSharp2;
        public const float GFlat2 = FSharp2;
        public const float AFlat2 = GSharp2;

        // Flats (Octave 3)
        public const float BFlat3 = ASharp3;
        public const float DFlat3 = CSharp3;
        public const float EFlat3 = DSharp3;
        public const float GFlat3 = FSharp3;
        public const float AFlat3 = GSharp3;


        public static float GetIntervalMultiplier(int numSemitones)
        {
            return Mathf.Pow(SemitoneMultiplier, numSemitones);
        }
    }
}