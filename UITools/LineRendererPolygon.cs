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
public class LineRendererPolygon : MonoBehaviour
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

        float stepAngle = 2f * Mathf.PI / NumVertices;
        float maxCornerRadius = Radius * Mathf.Sin(Mathf.PI / NumVertices);
        float cornerRadius = Mathf.Min(CornerRadius, maxCornerRadius - 1e-4f);

        var verts = new Vector2[NumVertices];
        for (int i = 0; i < NumVertices; i++)
        {
            float angle = i * stepAngle;
            verts[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Radius;
        }

        for (int i = 0; i < NumVertices; i++)
        {
            Vector2 prev = verts[(i - 1 + NumVertices) % NumVertices];
            Vector2 curr = verts[i];
            Vector2 next = verts[(i + 1) % NumVertices];

            Vector2 dirToPrev = (curr - prev).normalized;
            Vector2 dirToNext = (next - curr).normalized;

            float angleBetween = Vector2.Angle(-dirToPrev, dirToNext) * Mathf.Deg2Rad;
            float cutLength = cornerRadius / Mathf.Tan(angleBetween / 2f);

            cutLength = Mathf.Min(cutLength, Vector2.Distance(curr, prev) / 2f, Vector2.Distance(curr, next) / 2f);

            Vector2 cutA = curr - dirToPrev * cutLength;
            Vector2 cutB = curr + dirToNext * cutLength;

            Vector2 bisectorDir = new Vector2(-(dirToNext + dirToPrev).y, (dirToNext + dirToPrev).x).normalized;

            float bisectorAngle = Vector2.Angle(dirToPrev, -dirToNext) * Mathf.Deg2Rad / 2f;
            float arcCenterDist = cornerRadius / Mathf.Sin(bisectorAngle);

            Vector2 arcCenter = curr + bisectorDir * arcCenterDist;

            float startAngle = Mathf.Atan2(cutA.y - arcCenter.y, cutA.x - arcCenter.x);
            float endAngle = Mathf.Atan2(cutB.y - arcCenter.y, cutB.x - arcCenter.x);

            // Ensure proper direction
            if (Vector3.Cross(cutA - arcCenter, cutB - arcCenter).z < 0)
            {
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            for (int j = 0; j <= NumCornerPoints; j++)
            {
                float t = (float)j / NumCornerPoints;
                float angle = Mathf.LerpAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg, t) * Mathf.Deg2Rad;
                Vector2 arcPoint = arcCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * cornerRadius;
                points.Add(new Vector3(arcPoint.x, arcPoint.y, ZOffset));
            }
        }

        points.Add(points[0]); // close loop
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
}
#endif
#endif