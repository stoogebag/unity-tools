using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MeshFilter), typeof(PolygonCollider2D))]
public class PolygonColliderFromMesh : MonoBehaviour
{
    [Button]
  public void GenerateCollider()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) return;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        HashSet<KeyValuePair<int, int>> edges = new HashSet<KeyValuePair<int, int>>();

        // 1. Collect all edges. If we see an edge twice, it's internal; remove it.
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                int v1 = triangles[i + j];
                int v2 = triangles[i + (j + 1) % 3];

                // Create a unique key regardless of order
                var edge = v1 < v2 ? new KeyValuePair<int, int>(v1, v2) : new KeyValuePair<int, int>(v2, v1);

                if (!edges.Add(edge)) 
                    edges.Remove(edge);
            }
        }

        // 2. Map vertices to their connections for path following
        Dictionary<int, List<int>> adjacency = new Dictionary<int, List<int>>();
        foreach (var edge in edges)
        {
            if (!adjacency.ContainsKey(edge.Key)) adjacency[edge.Key] = new List<int>();
            if (!adjacency.ContainsKey(edge.Value)) adjacency[edge.Value] = new List<int>();
            adjacency[edge.Key].Add(edge.Value);
            adjacency[edge.Value].Add(edge.Key);
        }

        // 3. Trace the paths
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        List<List<Vector2>> allPaths = new List<List<Vector2>>();
        HashSet<int> visited = new HashSet<int>();

        foreach (var startNode in adjacency.Keys)
        {
            if (visited.Contains(startNode)) continue;

            List<Vector2> currentPath = new List<Vector2>();
            int curr = startNode;

            while (curr != -1)
            {
                visited.Add(curr);
                currentPath.Add(new Vector2(vertices[curr].x, vertices[curr].y));
                
                int next = -1;
                if (adjacency.ContainsKey(curr))
                {
                    foreach (int neighbor in adjacency[curr])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            next = neighbor;
                            break;
                        }
                    }
                }
                curr = next;
            }
            if (currentPath.Count > 2) allPaths.Add(currentPath);
        }

        // 4. Apply paths to collider
        poly.pathCount = allPaths.Count;
        for (int i = 0; i < allPaths.Count; i++)
        {
            poly.SetPath(i, allPaths[i].ToArray());
        }
    }
}