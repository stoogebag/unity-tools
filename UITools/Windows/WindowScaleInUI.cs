#if DOTWEEN
#if UNIRX
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace stoogebag.UITools.Windows
{
    public class WindowScaleInUI : MonoBehaviour, IWindowAnimation
    {
        private Vector3 _originalScale;
        [SerializeField] private Vector3 _closedScale = new Vector3(0.0f,0.0f,0.0f);
        [SerializeField] private Ease ease = Ease.InOutQuad;
        [SerializeField] private float time = 0.3f;
        [SerializeField] private bool AnimateOnClose = true;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public async UniTask<bool> Activate()
        {
            gameObject.SetActive(true);

            transform.localScale = _closedScale;
        
            await transform.DOScale(_originalScale, time).SetEase(ease).AsyncWaitForCompletion();

            return true;
        }

        public async UniTask<bool> Deactivate()
        {
            if (AnimateOnClose)
            {
                await transform.DOScale(_closedScale, time).SetEase(ease).AsyncWaitForCompletion();
                ResetScale();
            }
            else
            {
                ResetScale();
            }

            return true;
        }

        private void ResetScale()
        {
            transform.localScale = _originalScale;
        }
    }
}
#endif
#endif