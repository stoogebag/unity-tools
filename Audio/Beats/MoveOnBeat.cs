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
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private int maxSteps = -1;
        [SerializeField] private StepLoop loopBehaviour = StepLoop.Stop;

        private Tween _active;
        private Vector3 _startPos;
        private int _stepsTaken;
        private int _direction = 1;

        protected override void OnEnable()
        {
            _startPos = useLocalSpace ? transform.localPosition : transform.position;
            _stepsTaken = 0;
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
            Vector3 current = useLocalSpace ? transform.localPosition : transform.position;
            Vector3 next = current + step * _direction;

            bool hasLimit = maxSteps > 0;
            bool atLimit = hasLimit && _stepsTaken >= maxSteps;

            if (atLimit)
            {
                switch (loopBehaviour)
                {
                    case StepLoop.Stop:
                        return;
                    case StepLoop.LoopToStart:
                        next = _startPos;
                        _stepsTaken = 0;
                        _direction = 1;
                        break;
                    case StepLoop.PingPong:
                        _direction *= -1;
                        next = current + step * _direction;
                        _stepsTaken = 0;
                        break;
                }
            }

            _stepsTaken++;

            RestartTween(ref _active, () =>
            {
                if (useLocalSpace)
                    return transform.DOLocalMove(next, Mathf.Max(0.01f, duration)).SetEase(ease);
                return transform.DOMove(next, Mathf.Max(0.01f, duration)).SetEase(ease);
            });
        }
    }
}
