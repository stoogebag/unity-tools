using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ScaleOnHover : MonoBehaviour
{
    [SerializeField] private Vector3 ScaleFactor = new Vector3(1.1f, 1.1f, 1.1f);

    private ReactiveProperty<bool> _hovered = new();


    // Start is called before the first frame update
    void Start()
    {
        var scale = transform.localScale;

        var button = GetComponent<Button>();
        button.OnPointerEnterAsObservable().Subscribe(t =>
        {
            _hovered.Value = true;
        }).DisposeWith(this);
        button.OnPointerExitAsObservable().Subscribe(t =>
        {
            _hovered.Value = false;
        }).DisposeWith(this);

        _hovered.Subscribe(t =>
        {
            if (t == null) return;
            if (t)
            {

                transform.DOScale(scale.MultiplyPointwise(ScaleFactor), .2f).SetEase(Ease.OutBack).Play();
            }
            else
            {
                transform.DOScale(scale, .2f).SetEase(Ease.OutBack).Play();
            }
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
