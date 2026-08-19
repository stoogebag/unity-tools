using Sirenix.OdinInspector;
using UnityEngine;

public class EdgeColliderPointsFromLineRenderer : MonoBehaviour
{
    [Button]
    void BindPoints()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();

        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        Vector2[] points2D = new Vector2[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 pos = positions[i];
            if (lineRenderer.useWorldSpace)
                pos = transform.InverseTransformPoint(pos);
            points2D[i] = pos;
        }

        edgeCollider.points = points2D;
    }
}
