#if DOTWEEN
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag.UITools.Windows
{
    public class WindowSlideIn : MonoBehaviour, IWindowAnimation
    {
        /// <summary>
        /// optional. 
        /// </summary>
        [SerializeField]
        private Transform OffScreenPos;

        [SerializeField] private Vector3 Offset = new Vector3(0,10,0);

        [SerializeField] private bool useLocalPosition = true;

        public Vector3 _originalPos;
    
        [SerializeField] 
        private Ease ease = Ease.Linear;
        [SerializeField] 
        private float time = 0.5f;
        [SerializeField]
        private bool AnimateOnClose = true;

        private Vector3 _differenceVec;
        private Vector3 _offScreenPos;

        [Button]
        private void BakePositions()
        {
            _originalPos = useLocalPosition ? transform.localPosition : transform.position;
            if(OffScreenPos != null) _offScreenPos = OffScreenPos.position;
            else  _offScreenPos =  transform.localPosition + Offset;
        }

        private void Awake()
        {
            BakePositions();
        }

        public async UniTask<bool> Activate()
        {
            gameObject.SetActive(true);

            if(useLocalPosition) transform.localPosition = _offScreenPos;
            else transform.position = _offScreenPos;

            var tween = useLocalPosition ?  transform.DOLocalMove(_originalPos, time).SetEase(ease) : transform.DOMove(_originalPos, time).SetEase(ease);
            
            await tween.AsyncWaitForCompletion();

            return true;
        }

        public async UniTask<bool> Deactivate()
        {
            if (AnimateOnClose)
            {
                var tween = useLocalPosition
                    ? transform.DOLocalMove(_offScreenPos, time).SetEase(ease)
                    : transform.DOMove(_offScreenPos, time).SetEase(ease);
                    await tween.AsyncWaitForCompletion();
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
            transform.position = _originalPos;
        }

    }
}

#endif