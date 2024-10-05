using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using stoogebag.Utils;
using UnityEngine;

public class TimeScaleManager : Singleton<TimeScaleManager>
{
    private Tweener _tweener;

    public void SetTimeScale(float timeScale)
    {
        _tweener?.Kill();
        Time.timeScale = timeScale;
    }
    
    public void TweenTimeScale(float target, float duration)
    {
        // If a tween is already running, kill it
        _tweener?.Kill();

        // Start a new tween
        _tweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, target, duration);
    }

    public void StopTween()
    {
        // Kill the tween if it's running
        _tweener?.Kill();
    }
}
