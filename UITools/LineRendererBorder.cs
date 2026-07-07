#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererBorder : MonoBehaviour
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    
    public Vector2 Origin;
    public Vector2 Dimensions = Vector2.one;

    public float CornerRadius;
    public int NumCornerPoints = 1;

    public float ZOffset;

    private void Reset()
    {
        _disposable.Clear();
        gameObject.ObserveEveryValueChanged(t => Dimensions).Subscribe(v => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(t => Origin).Subscribe(v => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(t => CornerRadius).Subscribe(v => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(t => NumCornerPoints).Subscribe(v => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(t => ZOffset).Subscribe(v => Calculate()).AddTo(_disposable);

        gameObject.ObserveEveryValueChanged(t => t.transform.lossyScale).Subscribe(v => Calculate()).AddTo(_disposable);
    }


    [Button]
    void Calculate()
    {
        var lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;

        var points = new List<Vector2>();

        var w = Dimensions.x/2;
        var h = Dimensions.y/2;

        var sx = transform.lossyScale.x;
        var sy = transform.lossyScale.y;

        var rx = CornerRadius;
        var ry = Mathf.Approximately(sy, 0f) ? rx : CornerRadius * sx / sy;

        var tl = new Vector2(Origin.x - w + rx, Origin.y + h - ry);
        var tr = new Vector2(Origin.x + w - rx, Origin.y + h - ry);
        var bl = new Vector2(Origin.x - w + rx, Origin.y - h + ry);
        var br = new Vector2(Origin.x + w - rx, Origin.y - h + ry);

        //top right
        for (int i = 0; i < NumCornerPoints; i++)
        {
            var theta = 90f - (i * 90 * 1f / NumCornerPoints);
            var rad = theta * Mathf.Deg2Rad;
            var v = tr + new Vector2(rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
            points.Add(v);
        }

        //bottom right
        for (int i = 0; i < NumCornerPoints; i++)
        {
            var theta = - (i * 90 * 1f / NumCornerPoints);
            var rad = theta * Mathf.Deg2Rad;
            var v = br + new Vector2(rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
            points.Add(v);
        }

        //bottom left
        for (int i = 0; i < NumCornerPoints; i++)
        {
            var theta = -90f - (i * 90 * 1f / NumCornerPoints);
            var rad = theta * Mathf.Deg2Rad;
            var v = bl + new Vector2(rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
            points.Add(v);
        }

        //top left
        for (int i = 0; i < NumCornerPoints; i++)
        {
            var theta = -180f - (i * 90 * 1f / NumCornerPoints);
            var rad = theta * Mathf.Deg2Rad;
            var v = tl + new Vector2(rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
            points.Add(v);
        }

        points.Add(points[0]);
        
        lr.positionCount = points.Count;
        lr.SetPositions(points.Select(t=>t.WithZ(ZOffset)).ToArray());
    }
    

}
#endif
#endif