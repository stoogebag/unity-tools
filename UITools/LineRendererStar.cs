#if ODIN_INSPECTOR
#if UNIRX
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using stoogebag.Extensions;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererStar : MonoBehaviour
{
    private CompositeDisposable _disposable = new CompositeDisposable();

    public float Radius = 1f;
    [MinValue(3)] public int NumVertices = 6;

    public float CornerRadius = 0f;
    [MinValue(1)] public int NumCornerPoints = 3;

    public float ZOffset;

    private void Reset()
    {
        _disposable.Clear();
        gameObject.ObserveEveryValueChanged(_ => Radius).Subscribe(_ => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(_ => NumVertices).Subscribe(_ => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(_ => CornerRadius).Subscribe(_ => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(_ => NumCornerPoints).Subscribe(_ => Calculate()).AddTo(_disposable);
        gameObject.ObserveEveryValueChanged(_ => ZOffset).Subscribe(_ => Calculate()).AddTo(_disposable);
    }

    [Button]
    public void Calculate()
    { 
        
        
          var lr = GetComponent<LineRenderer>();
    var points = new List<Vector3>();

    int arms = NumVertices;
    int totalPoints = arms * 2;
    float step = Mathf.PI * 2f / totalPoints;

    float outer = Radius;
    float inner = Radius * 0.5f;

    // Precompute star vertices
    var verts = new Vector2[totalPoints];
    for (int i = 0; i < totalPoints; i++)
    {
        float angle = i * step;
        float r = (i % 2 == 0) ? outer : inner;
        verts[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
    }

    float maxRadius = Mathf.Min(outer, inner);
    float safeRadius = maxRadius * Mathf.Sin(Mathf.PI / totalPoints);
    float cr = Mathf.Min(CornerRadius, safeRadius - 1e-4f);

    for (int i = 0; i < totalPoints; i++)
    {
        Vector2 p0 = verts[(i - 1 + totalPoints) % totalPoints];
        Vector2 p1 = verts[i];
        Vector2 p2 = verts[(i + 1) % totalPoints];

        Vector2 v0 = (p0 - p1).normalized;
        Vector2 v1 = (p2 - p1).normalized;

        float angle = Vector2.Angle(v0, v1) * Mathf.Deg2Rad;
        float cutDist = cr / Mathf.Tan(angle / 2f);

        float d0 = Mathf.Min(cutDist, Vector2.Distance(p0, p1) * 0.49f);
        float d1 = Mathf.Min(cutDist, Vector2.Distance(p2, p1) * 0.49f);

        Vector2 cutA = p1 + v0 * d0;
        Vector2 cutB = p1 + v1 * d1;

        // arc center
        Vector2 bisector = ((v0 + v1) / 2f).normalized;
        float arcDist = cr / Mathf.Sin(angle / 2f);
        Vector2 center = p1 + bisector * arcDist;

        float a0 = Mathf.Atan2(cutA.y - center.y, cutA.x - center.x);
        float a1 = Mathf.Atan2(cutB.y - center.y, cutB.x - center.x);

        if (Vector3.Cross(cutA - center, cutB - center).z < 0)
            (a0, a1) = (a1, a0);

        for (int j = 0; j <= NumCornerPoints; j++)
        {
            float t = j / (float)NumCornerPoints;
            float angleLerp = Mathf.Lerp(a0, a1, t);
            Vector2 pt = center + new Vector2(Mathf.Cos(angleLerp), Mathf.Sin(angleLerp)) * cr;
            points.Add(new Vector3(pt.x, pt.y, ZOffset));
        }
    }

    points.Add(points[0]);
    lr.positionCount = points.Count;
    lr.SetPositions(points.ToArray());
    }
}
#endif
#endif