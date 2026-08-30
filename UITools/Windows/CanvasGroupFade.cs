#if DOTWEEN
#if UNIRX

using System;
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
        private float activateDelay = 0.1f;

        [SerializeField] 
        private float deactivateDelay = 0.1f;

        
        [SerializeField] 
        private float activateTime = 0.5f;
        [SerializeField] 
        private float deactivateTime = 0.5f;
        
        private CanvasGroup canvasGroup;
        private Tweener currentTween { get; set; }

        private void Init() //very annoying
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            var win = GetComponent<Window>();

            if (win.Active == ActiveState.Active || win.Active == ActiveState.Deactivating) canvasGroup.alpha = 1;
            if (win.Active == ActiveState.Inactive || win.Active == ActiveState.Activating) canvasGroup.alpha = 0;
        }

        public void SetParams(float activateDelay, float deactivateDelay, float activateTime, float deactivateTime)
        {
            this.activateDelay = activateDelay;
            this.deactivateDelay = deactivateDelay;
            this.activateTime = activateTime;
            this.deactivateTime = deactivateTime;
        }
        
        public async UniTask<bool> Activate()
        {
            //print("activating.");
            Init();
            gameObject.SetActive(true);
            
            //canvasGroup.a = _originalColor.WithAlpha(0);

            await UniTask.WaitForSeconds(activateDelay);
            currentTween = canvasGroup.DOFade(1f, activateTime).SetEase(ease).SetAutoKill(false);
            
            await currentTween.AsyncWaitForCompletion();
            
            if (currentTween.IsComplete())
            {
                currentTween.Kill();
                return true;
                // gameObject.SetActive(false);
                // ResetColor();
            }

            return false;
        }

        public async UniTask<bool> Deactivate()
        {
            //print("deactivating.");
            //Init();
            currentTween?.Kill(false);
            await UniTask.WaitForSeconds(deactivateDelay);
            currentTween = canvasGroup.DOFade(0, deactivateTime).SetEase(ease).SetAutoKill(false);

            await currentTween.AsyncWaitForCompletion();
            if (currentTween.IsComplete())
            {
                currentTween.Kill();
                return true;
            }

            return false;
        }

    }
}

#endif
#endif