using System.Collections.Generic;
using System.Linq;
using stoogebag;
using UnityEngine;

public class MeshBuilder
{
    private List<Triangle> Tris = new List<Triangle>();

    public Mesh GetMesh()
    {
        var vertToIndexDic = new Dictionary<Vector3, int>();

        var verts = new List<Vector3>();

        var triIndices = new List<int>(Tris.Count * 3);
        
        for (var i = 0; i < Tris.Count; i++)
        {
            var tri = Tris[i];

            var triVerts = new[] { tri.u, tri.v, tri.w };

            for (var j = 0; j < 3; j++)
            {
                var x = triVerts[j];
                if (vertToIndexDic.TryGetValue(x, out var index))
                {
                }
                else
                {
                    index = verts.Count;
                    verts.Add(x);
                }

                triIndices.Add(index);
            }

            
            
        }


        var output = new Mesh();
        output.SetVertices(verts);
        output.SetTriangles(triIndices, 0);
        return output;

    }


    public void AddTriangle(Triangle tri)
    {
        Tris.Add(tri);
    }

    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Tris.Add(new Triangle(a,b,d));
        Tris.Add(new Triangle(d,b,c));
    }
}

public static class MeshBuilderHelpers
{
    /// <summary>
    /// doesn't have to be an annulus, but any two paths of the SAME LENGTH, one inside the other. exotic paths might cause errors.
    /// intended to be used for an ellipse with a hole
    /// </summary>
    /// <param name="innerRing"></param>
    /// <param name="outerRing"></param>
    /// <returns></returns>
    public static void AddAnnulus2D(this MeshBuilder meshBuilder, List<Vector3> innerRing, List<Vector3> outerRing)
    {
        var count = innerRing.Count;
        for (var i = 0; i < count; i++)
        {
            var i2 = (i + 1) % count;

            var tri1 = new Triangle(innerRing[i], innerRing[i2], outerRing[i]);
            var tri2 = new Triangle(outerRing[i], innerRing[i2], outerRing[i2]);
            
            meshBuilder.AddTriangle(tri1);
            meshBuilder.AddTriangle(tri2);
        }
    }
    
    /// <summary>
    /// extruded in the y direction.
    /// </summary>
    /// <param name="meshBuilder"></param>
    /// <param name="innerRing"></param>
    /// <param name="outerRing"></param>
    /// <param name="thickness"></param>
    public static void AddAnnulus3D(this MeshBuilder meshBuilder, List<Vector3> innerRing, List<Vector3> outerRing, float thickness)
    {
        var count = innerRing.Count;
        var yVec = new Vector3(0, thickness, 0);
        for (var i = 0; i < count; i++)
        {
            var i2 = (i + 1) % count;

            var aTop = innerRing[i] +yVec;
            var bTop = innerRing[i2]+yVec;

            var cTop = outerRing[i2]+yVec;
            var dTop = outerRing[i] +yVec;
            
            var aBot = innerRing[i] ;
            var bBot = innerRing[i2];
                 
            var cBot = outerRing[i2];
            var dBot = outerRing[i] ;

            meshBuilder.AddQuad(aTop,bTop,cTop,dTop);
            meshBuilder.AddQuad(aBot,dBot,cBot,bBot);
            
            //innerWall
            meshBuilder.AddQuad(aTop,aBot,bBot,bTop);
            meshBuilder.AddQuad(dBot,dTop,cTop,cBot);

        }
    }
    
    
}