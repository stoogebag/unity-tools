using System;
using System.Collections.Generic;
using System.Linq;
using stoogebag;
using UnityEngine;

public class EdgeAngles :MonoBehaviour
{
    
    private void Reset()
    {
        GenerateMeshData();
    }

    /// <summary>
    /// We will assign a color to each Vertex in a Triangle on the object's mesh
    /// </summary>
    void GenerateMeshData()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        var info = new MeshInfo(mesh);
        info.InitialiseDictionaries();
        
        SetVertexColors(mesh, info);
            
        
        
    }

    private void SetVertexColors(Mesh mesh, MeshInfo meshInfo)
    {
        
        Color[] colorCoords = new[]
        {
            new Color(1, 0, 0),
            new Color(0, 1, 0),
            new Color(0, 0, 1),
        };

        
        Color32[] vertexColors = new Color32[mesh.vertices.Length];

        for (int i = 0; i < vertexColors.Length; i += 1)
        {
            var vert = mesh.vertices[i];

            var edges = meshInfo.vertexToEdgesDic[vert];
            var values = new float[3];
            for (var i1 = 0; i1 < edges.Count; i1++)
            {
                var edge = edges[i1];
                
                var tris = meshInfo.edgeToTrianglesDic[edge];
                if (tris.Count == 1) //boundary
                {
                    values[i1] = 1;
                    break;
                }

                var n1 = tris[0].GetNormal();
                var n2 = tris[1].GetNormal();

                var dot = n1.Dot(n2);
                values[i1] = (float)Math.Max(values[i1], 1-dot);
            }

            vertexColors[i] = new Color(values[0], values[1],values[2]);
        }

        mesh.colors32 = vertexColors;
    }
}

public class MeshInfo
{
    public Dictionary<Edge<Vector3>,List<Triangle>> edgeToTrianglesDic { get; private set; }
    public Dictionary<Vector3,List<Edge<Vector3>>> vertexToEdgesDic { get; private set; }
    public Triangle[] tris { get; }
    public Vector3[] verts { get; }
    public Edge<Vector3>[] edges { get; }

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

public class Edge<T>
{
    protected bool Equals(Edge<T> other)
    {
        return EqualityComparer<T>.Default.Equals(U, other.U) && EqualityComparer<T>.Default.Equals(V, other.V) ||
               EqualityComparer<T>.Default.Equals(U, other.V) && EqualityComparer<T>.Default.Equals(V, other.U);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Edge<T>)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(U, V) +HashCode.Combine(V,U) ;
    }

    public T U;
    public T V;
    
    public Edge(T u, T v)
    {
        U = u;
        V = v;
    }
}

public struct Triangle
{
    public int i;
    public int j;
    public int k;
    
    public Vector3 u;
    public Vector3 v;
    public Vector3 w;

    public Edge<Vector3> uv;
    public Edge<Vector3> vw;
    public Edge<Vector3> wu;

    public Vector3 GetNormal()
    {
        var A = v - u;
        var B = w - u;
        
        var Nx = A.y * B.z - A.z * B.y;
        var Ny = A.z * B.x - A.x * B.z;
        var Nz = A.x * B.y - A.y * B.x;

        return new Vector3(Nx, Ny, Nz);
    }
}