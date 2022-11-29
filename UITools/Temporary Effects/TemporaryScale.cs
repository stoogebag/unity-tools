#if DOTWEEN

using DG.Tweening;
using stoogebag_MonuMental.stoogebag.Extensions;
using stoogebag_MonuMental.stoogebag.UITools.Temporary_Effects;
using UniRx;
using UnityEngine;

public class TemporaryScale : TemporaryEffectBase
{
    public Vector3 scale = 2*Vector3.one;
    public float ScaleTime = .2f;

    public override void Activate()
    {
        //transform.localScale = transform.localScale.ScaleByVector(scale);
        
        var origScale = transform.localScale;
        var tween = transform.DOScale(transform.localScale.ScaleByVector(scale), ScaleTime);
        
        _disposable.Add(Disposable.Create(() =>
        {
            tween.Kill();
            transform.DOScale(origScale, ScaleTime);
        }));
    }

    public override void Deactivate()
    {
        _disposable.Clear();
    }
}

#endif