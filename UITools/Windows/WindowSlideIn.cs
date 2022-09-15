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
    public Vector3 RelativeOffScreenPos = Vector3.down * 10f;
    [SerializeField] private float time = 0.5f;

    [SerializeField]
    private bool AnimateOnClose = true;

    public async Task Activate()
    {
        gameObject.SetActive(true);
        var pos = transform.position;
        transform.position = pos + RelativeOffScreenPos;
        await transform.DOMove(pos, time).AsyncWaitForCompletion();
    }

    public async Task Deactivate()
    {
        if (AnimateOnClose)
        {
            var pos = transform.position;
            await transform.DOMove(pos + RelativeOffScreenPos, time).AsyncWaitForCompletion();
            gameObject.SetActive(false);
            transform.position = pos;
        }
        else gameObject.SetActive(false);


    }

    public bool Active { get; }
}

#endif