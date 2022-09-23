#if DOTWEEN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using stoogebag;
using UniRx;
public class WindowSlideIn : MonoBehaviour, IWindowAnimation
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
        _offScreenPos = OffScreenPos?.position ?? transform.position + Offset;
    }

    public async Task Activate()
    {
        gameObject.SetActive(true);

        transform.position = _offScreenPos;
        await transform.DOMove(_originalPos, time).SetEase(ease).AsyncWaitForCompletion();
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