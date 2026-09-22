using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class RotateOnBeat : OnBeatBehaviour
    {
        [SerializeField] private Vector3 rotationStep = new Vector3(0f, 90f, 0f);
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private bool useLocalSpace = true;

        private Tween _active;

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
                    return transform.DOLocalRotate(target, Mathf.Max(0.01f, duration), RotateMode.Fast).SetEase(ease);
                }

                var worldTarget = transform.eulerAngles + rotationStep;
                return transform.DORotate(worldTarget, Mathf.Max(0.01f, duration), RotateMode.Fast).SetEase(ease);
            });
        }
    }
}
