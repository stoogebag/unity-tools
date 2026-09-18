using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using stoogebag.Extensions;
using stoogebag.Utils;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class BeatManager : Singleton<BeatManager>
    {
        public IBeatProvider ActiveProvider { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            foreach (var provider in BeatProviders.SelectMany(t => t.GetComponentsWithInterface<IBeatProvider>()))
            {
                RegisterBeatProvider(provider);
            }
        }

        public void SetActiveProvider(IBeatProvider provider)
        {
            ActiveProvider = provider;
        }

        public void RegisterBeatProvider(IBeatProvider provider)
        {
            if (ActiveProvider == null) ActiveProvider = provider;
        }

        public async UniTask WaitFor(CuePoint cue, CancellationToken cancellationToken = default)
        {
            if (cue == CuePoint.Immediate) return;

            var provider = ActiveProvider;
            if (provider == null || !provider.IsRunning) return;

            if (cue == CuePoint.NextBeat)
            {
                try { await provider.OnAllBeats.First().ToUniTask(cancellationToken: cancellationToken); }
                catch (System.OperationCanceledException) { }
            }
            else
            {
                try { await provider.OnBeat(1f).First().ToUniTask(cancellationToken: cancellationToken); }
                catch (System.OperationCanceledException) { }
            }
        }

        [SerializeField]
        private List<GameObject> BeatProviders = new List<GameObject>();
    }

    public interface IBeatProvider
    {
        IObservable<BeatData> OnAllBeats { get; }
        IObservable<BeatData> OnBeat(float beatNumber);
        IObservable<BeatData> OnBeatAnticipated(float beatNumber, float leadTimeInSeconds);
        IObservable<BeatData> OnAllBeatsAnticipated(float leadTimeInSeconds);

        bool IsRunning { get; }
        float TimeToNextBeat { get; }
        float TimeToNextBar { get; }
        float TimeSinceLastBeat { get; }
        float TimeSinceLastBar { get; }

        float BeatIntervalSeconds { get; }
        float CurrentBeat { get; }
        int CurrentBar { get; }
        TimeSignature CurrentTimeSignature { get; }
        float BPM { get; }
    }

    public static class BeatStream
    {
        public const float BeatEpsilon = 0.001f;

        public static IObservable<BeatData> OnBeat(
            IObservable<BeatData> allBeats,
            float beatNumber,
            float leadTimeInSeconds,
            Func<float> beatInterval,
            Func<int> beatsPerBar)
        {
            float fraction = beatNumber - Mathf.Floor(beatNumber);
            bool isWhole = fraction < BeatEpsilon;

            float anchorBase = isWhole ? beatNumber - 1f : Mathf.Floor(beatNumber);
            float offset = isWhole ? 1f : fraction;
            bool wraps = anchorBase < 1f;

            return allBeats
                .Where(b =>
                {
                    float anchor = wraps ? beatsPerBar() : anchorBase;
                    return Mathf.Abs(b.BeatNumber - anchor) < BeatEpsilon;
                })
                .SelectMany(b =>
                {
                    float delay = offset * beatInterval() - leadTimeInSeconds;

                    if (delay <= 0f)
                        return Observable.Return(MakeTarget(b, beatNumber, wraps));

                    return Observable.Timer(TimeSpan.FromSeconds(delay))
                        .Select(_ => MakeTarget(b, beatNumber, wraps));
                });
        }

        public static IObservable<BeatData> OnAllBeatsAnticipated(
            IObservable<BeatData> allBeats,
            float leadTimeInSeconds,
            Func<float> beatInterval,
            Func<int> beatsPerBar)
        {
            return allBeats.SelectMany(b =>
            {
                int nextBeat = b.BeatNumber >= beatsPerBar() - BeatEpsilon
                    ? 1
                    : Mathf.FloorToInt(b.BeatNumber) + 1;
                bool wraps = nextBeat == 1;
                float delay = Mathf.Max(0f, beatInterval() - leadTimeInSeconds);
                var target = MakeTarget(b, nextBeat, wraps);

                if (delay <= 0f)
                    return Observable.Return(target);

                return Observable.Timer(TimeSpan.FromSeconds(delay)).Select(_ => target);
            });
        }

        private static BeatData MakeTarget(BeatData anchor, float beatNumber, bool wraps)
        {
            return new BeatData
            {
                Source = anchor.Source,
                BarNumber = anchor.BarNumber + (wraps ? 1 : 0),
                BeatNumber = beatNumber,
                CurrentTimeSignature = anchor.CurrentTimeSignature,
            };
        }
    }

    public class BeatData
    {
        public IBeatProvider Source;

        public int BarNumber;
        public float BeatNumber;

        public TimeSignature CurrentTimeSignature;
    }

    public enum BeatType
    {
        Strong,
        Weak,
    }

    public struct TimeSignature
    {
        public int BeatsPerBar;
        public int BeatUnit;
    }
}
