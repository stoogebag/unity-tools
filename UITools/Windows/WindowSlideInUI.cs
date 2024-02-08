#if DOTWEEN
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag.UITools.Windows
{
    public class WindowSlideInUI : MonoBehaviour, IWindowAnimation
    {
        /// <summary>
        /// optional. 
        /// </summary>
        [SerializeField]
        private Transform OffScreenPos;

        [SerializeField] private Vector3 Offset = new Vector3(0,-1000,0);

        public Vector3 _originalPos;
    
        [SerializeField] 
        private Ease ease = Ease.InOutQuad;
        [SerializeField] 
        private float time = 0.5f;
        [SerializeField]
        private bool AnimateOnClose = true;

        private Vector3 _differenceVec;
        private Vector3 _offScreenPos;

        private void Awake()
        {
            _originalPos = ((RectTransform)transform).anchoredPosition;
            if(OffScreenPos != null) _offScreenPos = OffScreenPos.position;
            else  _offScreenPos =  _originalPos + Offset;
        }

        public async UniTask<bool> Activate()
        {
            gameObject.SetActive(true);

            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = _offScreenPos;
        
        
            await DoTweenExtensions.AsyncWaitForCompletion(rect.DOAnchorPos3D(_originalPos, time, true).SetEase(ease));

            return true;
        }

        public async UniTask<bool> Deactivate()
        {
            if (AnimateOnClose)
            {
                var rect = GetComponent<RectTransform>();
                await DoTweenExtensions.AsyncWaitForCompletion(rect.DOAnchorPos3D(_offScreenPos, time).SetEase(ease));
                gameObject.SetActive(false);
                ResetPosition();
            }
            else
            {
                gameObject.SetActive(false);
                ResetPosition();
            }

            return true;
        }

        private void ResetPosition()
        {
            ((RectTransform)transform).anchoredPosition = _originalPos;
        }

    }
}

#endif