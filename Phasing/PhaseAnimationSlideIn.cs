using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhaseAnimationSlideIn : MonoBehaviour, IPhaseAnimation
    {
        [SerializeField] private float distance = 10f;
        [SerializeField] private Vector3 origin = Vector3.zero;
        [SerializeField] private float z = 0f;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private Ease ease = Ease.InOutQuad;

        public async UniTask Animate()
        {
            var targetPos = transform.position;

            var effectiveOrigin = origin;
            if (effectiveOrigin == targetPos)
                effectiveOrigin = targetPos + Vector3.down;

            var dirFromOrigin = (targetPos - effectiveOrigin).normalized;
            var startPos = targetPos + dirFromOrigin * distance;
            startPos.z += z;

            transform.position = startPos;
            await transform.DOMove(targetPos, duration).SetEase(ease);
        }
    }
}
