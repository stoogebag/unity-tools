﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathsHelpers
{

    public static List<Vector3> GetCircleXZ(float r, int numPoints)
    {
        return GetEllipseXZ(new Vector2(r, r), numPoints);
    }
    
    public static List<Vector3> GetEllipseXZ(Vector2 radii, int numPoints, float angleOffsetInRadians = 0f)
    {
        
        var points = new List<Vector3>();

        var angle = (float)(2f * Math.PI / numPoints);
        for (float theta = 0; theta <= 2 * Math.PI; theta += angle)
        {
            var x = radii.x * (float)Math.Cos(theta + angleOffsetInRadians);
            var z = radii.y * (float)Math.Sin(theta + angleOffsetInRadians);

            var p = new Vector3(x, 0, z);
            points.Add(p);
        }

        return points;
    } 
}
