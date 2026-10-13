using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Phasing
{
    public class PhaseAnimationAnimateScale : MonoBehaviour, IPhaseAnimation
    {
        [SerializeField] private float startScale = 0f;
        [SerializeField] private float endScale = 1f;
        [SerializeField] private float duration = 0.3f;

        public async UniTask Animate()
        {
            var originalScale = this.transform.localScale;

            transform.localScale = originalScale * startScale;
            await gameObject.transform.DOScale(originalScale * endScale, duration).SetEase(Ease.InOutQuad);
        }
    }
}
