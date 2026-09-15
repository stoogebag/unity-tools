using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    [CreateAssetMenu(fileName = "MusicProfile", menuName = "stooge/Music/Profile")]
    public class MusicProfile : ScriptableObject
    {
        [SerializeField] private EventReference musicEvent;
        [SerializeField] private List<MusicParamValue> startParams = new List<MusicParamValue>();
        [TextArea] [SerializeField] private string extras;

        public EventReference MusicEvent => musicEvent;
        public IReadOnlyList<MusicParamValue> StartParams => startParams;
        public string Extras => extras;
    }

    [Serializable]
    public class MusicParamValue
    {
        [ParamRef] public string Name;
        public float Value;
    }
}
