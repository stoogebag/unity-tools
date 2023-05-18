using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stoogebag.Rendering
{
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
            //Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            //MeshDataBuilder.SplitMesh(mesh);
        
            //var info = new MeshInfo(mesh);
            //info.InitialiseDictionaries();
        
            //SetVertexColors(mesh, info, edge =>
            //{
            //    var tris = info.edgeToTrianglesDic[edge];
            //    if (tris.Count == 1) return true;

            //    return false;
            //});
            
        
        
        }

        private void SetVertexColors(Mesh mesh, MeshInfo meshInfo, Func<Edge<Vector3>, bool> edgeIncludeFunc )
        {
            Color[] colorCoords = new[]
            {
                new Color(1, 0, 0),
                new Color(0, 1, 0),
                new Color(0, 0, 1),
                new Color(1, 0, 0,0),
                new Color(0, 1, 0,0),
                new Color(0, 0, 1,0),
            };

            Color32[] vertexColors = new Color32[mesh.vertices.Length];

            for (int i = 0; i < vertexColors.Length; i += 1)
            {
                var u = mesh.vertices[i];
                //var v = mesh.vertices[i+1];
                //var w = mesh.vertices[i+2];
            
                var edgesOnU = meshInfo.vertexToEdgesDic[u];
                if (edgesOnU.All(t => edgeIncludeFunc(t)))
                {
                    vertexColors[i] = colorCoords[i%3];
                }
                else
                {
                    vertexColors[i] = colorCoords[i%3 + 3];
                }
            
            
            
            
                // vertexColors[i] = colorCoords[0];
                // vertexColors[i + 1] = colorCoords[1];
                // vertexColors[i + 2] = colorCoords[5];

            }

            mesh.colors32 = vertexColors;
            return;
            // Color[] colorCoords = new[]
            // {
            //     new Color(1, 0, 0),
            //     new Color(0, 1, 0),
            //     new Color(0, 0, 1),
            // };
            //
            //
            // Color32[] vertexColors = new Color32[mesh.vertices.Length];
            //
            // for (int i = 0; i < vertexColors.Length; i += 1)
            // {
            //     var vert = mesh.vertices[i];
            //
            //     var edges = meshInfo.vertexToEdgesDic[vert];
            //     var values = new float[3];
            //     for (var i1 = 0; i1 < edges.Count; i1++)
            //     {
            //         var edge = edges[i1];
            //         
            //         var tris = meshInfo.edgeToTrianglesDic[edge];
            //         if (tris.Count == 1) //boundary
            //         {
            //             values[i1] = 1;
            //             break;
            //         }
            //
            //         var n1 = tris[0].GetNormal();
            //         var n2 = tris[1].GetNormal();
            //
            //         var dot = n1.Dot(n2);
            //         values[i1] = (float)Math.Max(values[i1], 1-dot);
            //     }
            //
            //     vertexColors[i] = new Color(values[0], values[1],values[2]);
            // }
            //
            // mesh.colors32 = vertexColors;
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
            return U.GetHashCode() ^ V.GetHashCode();
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

        public Triangle(Vector3 u, Vector3 v, Vector3 w) : this()
        {
            this.u = u;
            this.v = v;
            this.w = w;
        }

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
}