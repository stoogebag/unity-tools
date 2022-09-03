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

    public async Task Activate()
    {
        gameObject.SetActive(true);
        var pos = transform.position;
        transform.position = pos + RelativeOffScreenPos;
        await transform.DOMove(pos, time).AsyncWaitForCompletion();
    }

    public async Task Deactivate()
    {
        var pos = transform.position;
        await transform.DOMove(pos + RelativeOffScreenPos, time).AsyncWaitForCompletion();
        gameObject.SetActive(false);
        transform.position = pos;
    }

    public bool Active { get; }
}
