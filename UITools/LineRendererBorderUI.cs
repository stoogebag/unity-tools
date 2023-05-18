using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

#if UI_EXTENSIONS


using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRenderer))]
public class LineRendererBorderUI : MonoBehaviour
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
        gameObject.ObserveEveryValueChanged(t => Dimensions).Subscribe(v => Calculate());
        gameObject.ObserveEveryValueChanged(t => Origin).Subscribe(v => Calculate());
        gameObject.ObserveEveryValueChanged(t => CornerRadius).Subscribe(v => Calculate());
        gameObject.ObserveEveryValueChanged(t => NumCornerPoints).Subscribe(v => Calculate());
        gameObject.ObserveEveryValueChanged(t => ZOffset).Subscribe(v => Calculate());
    }


    [Button]
    void Calculate()
    {
        var lr = GetComponent<UILineRenderer>();
        var points = new List<Vector2>();

        var w = Dimensions.x/2;
        var h = Dimensions.y/2;
        
        var tl = new Vector2(Origin.x - w + CornerRadius, Origin.y + h - CornerRadius);
        var tr = new Vector2(Origin.x + w - CornerRadius, Origin.y + h - CornerRadius);
        var bl = new Vector2(Origin.x - w + CornerRadius, Origin.y - h + CornerRadius);
        var br = new Vector2(Origin.x + w - CornerRadius, Origin.y - h + CornerRadius);

        //top right
        for (int i = 0; i <= NumCornerPoints; i++)
        {
            var theta = 90f - (i * 90 * 1f / NumCornerPoints);
            var rot = VectorExtensions.FromPolarDegrees(CornerRadius, theta);
            var v = tr + rot;
            points.Add(v);
        }
        
        //bottom right
        for (int i = 0; i <= NumCornerPoints; i++)
        {
            var theta = - (i * 90 * 1f / NumCornerPoints);
            var rot = VectorExtensions.FromPolarDegrees(CornerRadius, theta);
            var v = br + rot;
            points.Add(v);
        }
        
        //bottom left
        for (int i = 0; i <= NumCornerPoints; i++)
        {
            var theta = -90f - (i * 90 * 1f / NumCornerPoints);
            var rot = VectorExtensions.FromPolarDegrees(CornerRadius, theta);
            var v = bl + rot;
            points.Add(v);
        }
        
        //top left
        for (int i = 0; i <= NumCornerPoints; i++)
        {
            var theta = -180f - (i * 90 * 1f / NumCornerPoints);
            var rot = VectorExtensions.FromPolarDegrees(CornerRadius, theta);
            var v = tl + rot;
            points.Add(v);
        }

        points.Add(points[0]);


        lr.Points = points.ToArray();
        // lr.positionCount = points.Count;
        // lr.SetPositions(points.Select(t=>t.WithZ(ZOffset)).ToArray());
    }
    

}
#endif