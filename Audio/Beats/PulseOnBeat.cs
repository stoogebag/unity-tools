using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class PulseOnBeat : OnBeatBehaviour
    {
        [SerializeField] private float squashScale = 0.95f;
        [SerializeField] private float punchScale = 1.2f;
        [SerializeField] private float releaseDuration = 0.3f;
        [SerializeField] private Ease squashEase = Ease.InQuad;
        [SerializeField] private Ease releaseEase = Ease.OutCubic;

        private Tween _active;
        private Vector3 _baseScale;

        protected override void OnEnable()
        {
            _baseScale = transform.localScale;
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
                var seq = DOTween.Sequence();
                seq.Append(transform.DOScale(_baseScale * squashScale, Mathf.Max(0.01f, LeadTimeSeconds)).SetEase(squashEase));
                seq.AppendCallback(() => transform.localScale = _baseScale * punchScale);
                seq.Append(transform.DOScale(_baseScale, Mathf.Max(0.01f, releaseDuration)).SetEase(releaseEase));
                return seq;
            });
        }
    }
}
