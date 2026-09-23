using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class FollowOnBeat : OnBeatBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool targetPlayer;
        [SerializeField] private float stepSize = 1f;
        [SerializeField] private bool faceTarget;
        [SerializeField] private Ease moveEase = Ease.InOutCubic;
        [SerializeField] private Ease turnEase = Ease.InOutCubic;

        private Tween _active;
        private Tween _turn;

        protected override void Start()
        {
            if (target == null && targetPlayer)
                target = FindPlayerTarget();

            base.Start();
        }

        protected override void OnEnable()
        {
            if (stepSize < 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(stepSize),
                    $"[FollowOnBeat] stepSize must be >= 0 (got {stepSize}).");

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_active != null && _active.IsActive())
                _active.Kill();
            _active = null;
            if (_turn != null && _turn.IsActive())
                _turn.Kill();
            _turn = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (target == null && !targetPlayer)
                Debug.LogWarning("[FollowOnBeat] No target assigned and targetPlayer is false.", this);
        }

        private static Transform FindPlayerTarget()
        {
            var type = Type.GetType("PlayerControllerFishnet")
                ?? AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType("PlayerControllerFishnet", false))
                    .FirstOrDefault(t => t != null);

            if (type == null)
                return null;

            return FindFirstObjectByType(type) is Component component ? component.transform : null;
        }

        protected override void OnBeatTriggered(BeatData beat)
        {
            if (target == null)
                return;

            Vector3 origin = transform.position;
            Vector3 toTarget = target.position - origin;
            toTarget.z = 0f;

            if (toTarget.sqrMagnitude <= Mathf.Epsilon)
                return;

            Vector2 direction = toTarget.normalized;

            Vector3 destination = origin + (Vector3)(direction * stepSize);
            RestartTween(ref _active, () =>
                transform.DOMove(destination, LeadTimeSeconds).SetEase(moveEase));

            if (!faceTarget)
                return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RestartTween(ref _turn, () =>
                transform.DORotate(new Vector3(0f, 0f, angle), LeadTimeSeconds, RotateMode.Fast).SetEase(turnEase));
        }

        private void Update()
        {
            //todo: make this better lmao
            if (target == null && targetPlayer)
                target = FindPlayerTarget();
        }
    }
}
