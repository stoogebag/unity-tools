using System;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class RotateOnBeat : OnBeatBehaviour
    {
        [SerializeField] private Vector3 rotationStep = new Vector3(0f, 0f, 90f);
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private bool useLocalSpace = true;

        private Tween _active;

        protected override void OnEnable()
        {
            if (duration < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(duration),
                    $"[RotateOnBeat] duration must be >= 0 (got {duration}s).");

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_active != null && _active.IsActive())
                _active.Kill();
            _active = null;
        }

        protected override void OnBeatTriggered(BeatData beat)
        {
            RestartTween(ref _active, () =>
            {
                if (useLocalSpace)
                {
                    var target = transform.localEulerAngles + rotationStep;
                    return transform.DOLocalRotate(target, duration, RotateMode.Fast).SetEase(ease);
                }

                var worldTarget = transform.eulerAngles + rotationStep;
                return transform.DORotate(worldTarget, duration, RotateMode.Fast).SetEase(ease);
            });
        }
    }
}
