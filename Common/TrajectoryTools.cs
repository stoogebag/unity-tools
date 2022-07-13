using System;
using UnityEngine;

public class TrajectoryTools
{
    public class TrajectoryInfo
    {
        public Vector3 InitialPosition;
        public float InitialSpeed;
        public Vector3 ConstantForce;
        public Vector3 FinalPosition;

        public Vector3 InitialVelocity;
        public float ArrivalTime;
    }


    public static float GetArrivalTimes(TrajectoryInfo info)
    {
        var D = info.FinalPosition - info.InitialPosition;
        var F = info.ConstantForce;
        var s = info.InitialSpeed;
        
        //we compute a quadratic polynomial in t^2 with coefficients A,B,C
        var A = F.x * F.x / 4 + F.y * F.y / 4 + F.z * F.z / 4;
        var B = -s * s - (F.x * D.x + F.y * D.y + F.z * D.z);
        var C = D.x * D.x + D.y * D.y + D.z * D.z;

        var (tSquared1, tSquared2) = MathsExtensions.QuadraticFormula(A, B, C);

        var smallTsq = Math.Min(tSquared1, tSquared2);
        return (float)Math.Sqrt(smallTsq);
    }

    public static void Calculate(TrajectoryInfo info)
    {
        var D = info.FinalPosition - info.InitialPosition;
        var F = info.ConstantForce;
        
        var t = GetArrivalTimes(info);
        info.ArrivalTime = t;
        info.InitialVelocity = 1 / t * D - t / 2* F;
    }

    public static Vector3 GetPosition(TrajectoryInfo info, float t)
    {
        if (info.ArrivalTime == 0)
            throw new Exception("run calculate() first! or your target is the same as your start point!");

        return info.InitialPosition + t * info.InitialVelocity + t * t * 0.5f * info.ConstantForce;
    }
    
    
    
}