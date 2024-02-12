#if DOTWEEN
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag.UITools.Windows
{
    public class CanvasGroupFade : MonoBehaviour, IWindowAnimation
    {
    
        [SerializeField] 
        private Ease ease = Ease.Linear;
        [SerializeField] 
        private float time = 0.5f;
        [SerializeField]
        private bool AnimateOnClose = true;
        
        private CanvasGroup canvasGroup;
        private Tweener currentTween { get; set; }

        private void Init() //very annoying
        {
            if (canvasGroup != null) return;
            canvasGroup = GetComponent<CanvasGroup>();

            var win = GetComponent<Window>();
            canvasGroup.alpha = win.Active == ActiveState.Active ? 1 : 0;
        }

        public async UniTask<bool> Activate()
        {
            //print("activating.");
            Init();
            gameObject.SetActive(true);
            
            //canvasGroup.a = _originalColor.WithAlpha(0);

            currentTween = canvasGroup.DOFade(1f, time).SetEase(ease).SetAutoKill(false);
            
            await currentTween.AsyncWaitForCompletion();
            
            if (currentTween.IsComplete())
            {
                return true;
                // gameObject.SetActive(false);
                // ResetColor();
            }

            return false;
        }

        public async UniTask<bool> Deactivate()
        {
            //print("deactivating.");
            Init();
            if (AnimateOnClose)
            {

                currentTween?.Kill(false);
                currentTween = canvasGroup.DOFade(0, time).SetEase(ease).SetAutoKill(false);
                
                await currentTween.AsyncWaitForCompletion();
                if (currentTween.IsComplete())
                {
                    gameObject.SetActive(false);
                    return true;
                }

                return false;
            }
            else
            {
                gameObject.SetActive(false);
                return true;
            }
        }

    }
}

#endif