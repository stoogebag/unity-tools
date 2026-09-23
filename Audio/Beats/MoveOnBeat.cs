using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class MoveOnBeat : OnBeatBehaviour
    {
        public enum StepLoop
        {
            Stop,
            LoopToStart,
            PingPong,
        }

        [SerializeField] private Vector3 step = Vector3.right;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private int maxSteps = -1;
        [SerializeField] private StepLoop loopBehaviour = StepLoop.Stop;

        private Tween _active;
        private Vector3 _startPos;
        private int _stepIndex;
        private int _direction = 1;

        protected override void OnEnable()
        {
            _startPos = useLocalSpace ? transform.localPosition : transform.position;
            _stepIndex = 0;
            _direction = 1;
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
            int next = _stepIndex + _direction;

            if (maxSteps > 0)
            {
                if (loopBehaviour == StepLoop.PingPong)
                {
                    if (next > maxSteps || next < 0)
                    {
                        _direction *= -1;
                        next = _stepIndex + _direction;
                    }
                }
                else if (next > maxSteps)
                {
                    if (loopBehaviour == StepLoop.Stop)
                        return;

                    next = 0;
                }
            }

            _stepIndex = next;

            Vector3 target = _startPos + step * _stepIndex;

            RestartTween(ref _active, () =>
            {
                if (useLocalSpace)
                    return transform.DOLocalMove(target, LeadTimeSeconds).SetEase(ease);
                return transform.DOMove(target, LeadTimeSeconds).SetEase(ease);
            });
        }
    }
}
