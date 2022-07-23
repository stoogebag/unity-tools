using System;
using stoogebag;
using UnityEngine;

public static class MathsExtensions
{
    public static (float, float) QuadraticFormula(float a, float b, float c)
    {
        if (a == 0)
        {
            if (b == 0) return (float.NaN, float.NaN);
            else return (-c / b, -c/b);
        }
        
        var disc = Discriminant(a, b, c);
        return ((-b + (float)Math.Sqrt(disc)) / (2 * a),(-b - (float)Math.Sqrt(disc)) / (2 * a));
    }

    public static float Discriminant(float a, float b, float c)
    {
        return b * b - 4 * a * c;
    }

    
    public static Vector3 NearestPointOnLine(Vector3 a, Vector3 b, Vector3 X)
    {
        //a is a point on the line. b is the direction. x the other point. returns nearest point on line to X.
        
        //line is a+tb
        //(x-(a+tb)) is orthogonal to b. solve for t
        //ie x.b-a.b-tb.b == 0
        var bb = b.Dot(b);
        if (bb == 0) throw new Exception("b cannot be zero! that's no line");

        var t = (X.Dot(b) - a.Dot(b)) / bb;
        var result = a + t * b;
        return result;
    }
    
    
}
