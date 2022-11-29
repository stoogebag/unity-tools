#if DOTWEEN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG;
using stoogebag_MonuMental.stoogebag.UITools.Windows;
using UnityEngine;
using UniRx;
public class WindowSlideInUI : MonoBehaviour, IWindowAnimation
{
    /// <summary>
    /// optional. 
    /// </summary>
    [SerializeField]
    private Transform OffScreenPos;

    [SerializeField] private Vector3 Offset = new Vector3(0,10,0);

    public Vector3 _originalPos;
    
    [SerializeField] 
    private Ease ease = Ease.Linear;
    [SerializeField] 
    private float time = 0.5f;
    [SerializeField]
    private bool AnimateOnClose = true;

    private Vector3 _differenceVec;
    private Vector3 _offScreenPos;

    private void Awake()
    {
        _originalPos = transform.position;
        if(OffScreenPos != null) _offScreenPos = OffScreenPos.position;
            else  _offScreenPos =  transform.position + Offset;
    }

    public async Task Activate()
    {
        gameObject.SetActive(true);

        var rect = GetComponent<RectTransform>();
        rect.position = _offScreenPos;
        
        
        await rect.DOAnchorPos3D(_originalPos, time, true).SetEase(ease).AsyncWaitForCompletion();
    }

    public async Task Deactivate()
    {
        if (AnimateOnClose)
        {
            await transform.DOMove(_offScreenPos, time).SetEase(ease).AsyncWaitForCompletion();
            gameObject.SetActive(false);
            ResetPosition();
        }
        else
        {
            gameObject.SetActive(false);
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        transform.position = _originalPos;
    }

}

#endif