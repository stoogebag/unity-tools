using System.Collections.Generic;
using System.Linq;
using stoogebag;
using UnityEngine;

public class MeshInfo
{
    public Dictionary<Edge<Vector3>,List<Triangle>> edgeToTrianglesDic { get; private set; }
    public Dictionary<Vector3,List<Edge<Vector3>>> vertexToEdgesDic { get; private set; }
    public Triangle[] tris { get; }
    public Vector3[] verts { get; }
    public Edge<Vector3>[] edges { get; }

    public MeshInfo()
    {
    }

    public MeshInfo(Mesh mesh)
    {
        tris = new Triangle[mesh.triangles.Length / 3];
        verts = mesh.vertices.ToArray();
        
        for (var t = 0; t < mesh.triangles.Length; t+=3)
        {
            var i = mesh.triangles[t];
            var j = mesh.triangles[t+1];
            var k = mesh.triangles[t+2];

            var u = verts[i];
            var v = verts[j];
            var w = verts[k];
            
            var uv = new Edge<Vector3>(u,v);
            var vw = new Edge<Vector3>(v,w);
            var wu = new Edge<Vector3>(w,u);

            var tri = new Triangle()
            {
                i = i,
                j = j,
                k = k,
                u = u,
                v = v,
                w = w,
                uv = uv,
                vw = vw,
                wu = wu,
            };

            tris[t / 3] = tri;
        }
    }
    public void InitialiseDictionaries()
    {
        edgeToTrianglesDic = new Dictionary<Edge<Vector3>, List<Triangle>>();

        foreach (var t in tris)
        {
            var list = edgeToTrianglesDic.TryGetOrAdd(t.uv, () => new List<Triangle>());
            list.Add(t);

            list = edgeToTrianglesDic.TryGetOrAdd(t.vw, () => new List<Triangle>());
            list.Add(t);

            list = edgeToTrianglesDic.TryGetOrAdd(t.wu, () => new List<Triangle>());
            list.Add(t);
        }

        vertexToEdgesDic = new Dictionary<Vector3, List<Edge<Vector3>>>();
        foreach (var edge in edgeToTrianglesDic.Keys)
        {
            var list = vertexToEdgesDic.TryGetOrAdd(edge.U, () => new List<Edge<Vector3>>());
            list.Add(edge);
            list = vertexToEdgesDic.TryGetOrAdd(edge.V, () => new List<Edge<Vector3>>());
            list.Add(edge);
        }


    }
}