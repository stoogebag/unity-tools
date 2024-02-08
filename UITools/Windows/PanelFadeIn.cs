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
    public class PanelFadeIn : MonoBehaviour, IWindowAnimation
    {
    
        [SerializeField] 
        private Ease ease = Ease.Linear;
        [SerializeField] 
        private float time = 0.5f;
        [SerializeField]
        private bool AnimateOnClose = true;

        private Color _originalColor;
        
        private Image _panel;
        private TweenerCore<Color,Color,ColorOptions> currentTween;

        private void Init() //very annoying
        {
            if (_panel != null) return;
            _panel = GetComponent<Image>();
            _originalColor = _panel.color;
        }

        public async UniTask<bool> Activate()
        {
            print("activating.");
            Init();
            gameObject.SetActive(true);
            
            _panel.color = _originalColor.WithAlpha(0);
            currentTween = _panel.DOColor(_originalColor, time).SetEase(ease);
            
            await DoTweenExtensions.AsyncWaitForCompletion(currentTween);
            
            
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
            
            print("deactivating.");
            Init();
            if (AnimateOnClose)
            {

                currentTween?.Kill(false);
                currentTween = _panel.DOColor(_originalColor.WithAlpha(0), time).SetEase(ease);
                
                await DoTweenExtensions.AsyncWaitForCompletion(currentTween);
                if (currentTween.IsComplete())
                {
                    gameObject.SetActive(false);
                    ResetColor();

                    return true;
                }

                return false;
            }
            else
            {
                gameObject.SetActive(false);
                ResetColor();
                return true;
            }
        }

        private void ResetColor()
        {
            _panel.color = _originalColor;
        }

    }
}

#endif