using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class FollowPathOnBeat : OnBeatBehaviour
    {
        public enum PathLoop
        {
            Loop,
            PingPong,
            StopAtEnd,
        }

        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private PathLoop loopBehaviour = PathLoop.Loop;
        [SerializeField] private int startIndex = 0;

        private Tween _active;
        private int _currentIndex;
        private int _direction = 1;

        protected override void OnEnable()
        {
            _direction = 1;
            _currentIndex = waypoints != null && waypoints.Count > 0
                ? Mathf.Clamp(startIndex, 0, waypoints.Count - 1)
                : 0;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_active != null && _active.IsActive())
                _active.Kill();
            _active = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (waypoints == null || waypoints.Count == 0)
                Debug.LogWarning("[FollowPathOnBeat] No waypoints assigned.", this);
        }

        protected override void OnBeatTriggered(BeatData beat)
        {
            if (waypoints == null || waypoints.Count == 0)
                return;

            int next = AdvanceIndex();
            if (next < 0)
                return;

            _currentIndex = next;
            Transform target = waypoints[_currentIndex];
            if (target == null)
                return;

            Vector3 destination = target.position;

            RestartTween(ref _active, () =>
                transform.DOMove(destination, Mathf.Max(0.01f, duration)).SetEase(ease));
        }

        private int AdvanceIndex()
        {
            int count = waypoints.Count;
            if (count <= 1)
                return 0;

            switch (loopBehaviour)
            {
                case PathLoop.Loop:
                    return (_currentIndex + 1) % count;
                case PathLoop.StopAtEnd:
                    return _currentIndex >= count - 1 ? -1 : _currentIndex + 1;
                case PathLoop.PingPong:
                default:
                    int next = _currentIndex + _direction;
                    if (next >= count || next < 0)
                    {
                        _direction *= -1;
                        next = _currentIndex + _direction;
                    }
                    return next;
            }
        }
    }
}
