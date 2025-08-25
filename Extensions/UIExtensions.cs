using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace stoogebag.Extensions
{
    public static class UIExtensions
    {
        //this is not for ui, move at some point.
        public static IObservable<bool> IsMouseOverObservable(this Component button)
        {
            var enter = button.OnMouseOverAsObservable().Select(_ => true);
            var exit = button.OnMouseExitAsObservable().Select(_ => false);
            return enter.Merge(exit).StartWith(false).DistinctUntilChanged();
        }
        public static IObservable<bool> IsPointerOverObservable(this UIBehaviour button)
        {
            var enter = button.OnPointerEnterAsObservable().Select(_ => true);
            var exit = button.OnPointerExitAsObservable().Select(_ => false);
            return enter.Merge(exit).StartWith(false).DistinctUntilChanged();
        }
        
        
    }
}