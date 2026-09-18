using FMODUnity;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    [CreateAssetMenu(fileName = "FillProfile", menuName = "stooge/Music/Fill Profile")]
    public class FillProfile : ScriptableObject
    {
        [SerializeField] private EventReference fillEvent;
        [SerializeField] private float secondsUntilNextTrack;
        [SerializeField] private float keepExistingSeconds;

        public EventReference FillEvent => fillEvent;

        // Seconds after the fill starts before the next track should play.
        // NOT the full length of the fill — its tail may ring on over the downbeat.
        public float SecondsUntilNextTrack => secondsUntilNextTrack;

        // How long the existing track keeps running after the fill starts, before it fades.
        public float KeepExistingSeconds => keepExistingSeconds;
    }
}