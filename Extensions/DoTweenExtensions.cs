#if DOTWEEN
using DG.Tweening;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class DoTweenExtensions
    {
        //
        //
        // public static async System.Threading.Tasks.Task AsyncWaitForCompletion(this Tween t)
        // {
        //     if (!t.active) {
        //         //if (Debugger.logPriority > 0) Debugger.LogInvalidTween(t);
        //         return;
        //     }
        //     while (t.active && !t.IsComplete()) await System.Threading.Tasks.Task.Yield();
        // }

        public static Tween DoColorProperty(this Material material, string materialPropertyName, Color targetColor, float duration)
        {
            return DOTween.To(() => material.GetColor(materialPropertyName), x => material.SetColor(materialPropertyName, x), targetColor, duration);
        }
        
    }
}

#endif