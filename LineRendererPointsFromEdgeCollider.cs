using Sirenix.OdinInspector;
using UnityEngine;

public class LineRendererPointsFromEdgeCollider : MonoBehaviour
{
    [Button]
    void BindPoints()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        Vector2[] points2D = edgeCollider.points;
        Vector3[] points3D = new Vector3[points2D.Length];

        for (int i = 0; i < points2D.Length; i++)
        {
            points3D[i] = points2D[i];
        }

        lineRenderer.positionCount = points3D.Length;
        lineRenderer.SetPositions(points3D);

    }
}
