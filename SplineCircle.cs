#if DREAMTECK_SPLINES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;
using stoogebag;

public class SplineCircle : MonoBehaviour //actually an ellipse XD
{
    public Vector2 Centre = Vector2.zero;
    public Vector2 Radii = new Vector2(1,1);

    public int numPoints = 10;
    
    [Button("apply")]
    void Apply()
    {
        var points = new List<Vector3>();

        var angle = (float)(2f * Math.PI / numPoints);
        for (float theta = 0; theta <= 2 * Math.PI; theta += angle)
        {
            var x = Radii.x * (float)Math.Cos(theta);
            var z = Radii.y * (float)Math.Sin(theta);

            var p = new Vector3(x, 0, z);
            points.Add(p);
        }

        points.Add(points[0]);

        var spline = GetComponent<SplineComputer>();
        //spline.cl.isClosed = true;
        spline.SetPoints(points.Select(t=>new SplinePoint(t)).ToArray());
        
    }
}

#endif