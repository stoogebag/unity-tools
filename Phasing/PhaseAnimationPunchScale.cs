using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhaseAnimationPunchScale : MonoBehaviour, IPhaseAnimation
    {
        [SerializeField] private float scaleFactor;
        [SerializeField] private float duration;

        public async UniTask Animate()
        {
            await gameObject.transform.DOPunchScale(Vector3.one * scaleFactor, duration);
        }
    }
}
