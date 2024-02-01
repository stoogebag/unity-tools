using System;
using UnityEngine;

namespace stoogebag.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        
        public static Vector2 ToVector2XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static float DistanceToSquared(this Vector3 v, Vector3 w)
        {
            return (w - v).sqrMagnitude;
        }

        public static float DistanceTo(this Vector3 v, Vector3 w)
        {
            return (w - v).magnitude;
        }

        public static float DistanceToSquared(this Vector2 v, Vector2 w)
        {
            return (w - v).sqrMagnitude;
        }

        public static float DistanceTo(this Vector2 v, Vector2 w)
        {
            return (w - v).magnitude;
        }

        public static float DistanceToSquared(this Vector4 v, Vector4 w)
        {
            return (w - v).sqrMagnitude;
        }

        public static float DistanceTo(this Vector4 v, Vector4 w)
        {
            return (w - v).magnitude;
        }

        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 WithZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
        public static Vector4 WithW(this Vector3 v, float w)
        {
            return new Vector4(v.x, v.y, v.z, w);
        }

        public static Vector2 Rounded(this Vector2 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
            return new Vector2((float)Math.Round(v.x*pow)/pow, (float)Math.Round(v.y*pow)/pow);
        }
        public static Vector3 Rounded(this Vector3 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
            return new Vector3((float)Math.Round(v.x*pow)/pow, (float)Math.Round(v.y*pow)/pow,(float)Math.Round(v.z*pow)/pow);
        }
        public static Vector3Int RoundedToInt(this Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y),Mathf.RoundToInt(v.z));
        }
        public static Vector2 Floor(this Vector2 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
            return new Vector2((float)Math.Floor(v.x*pow)/pow, (float)Math.Floor(v.y*pow)/pow);
        }
        public static Vector3 Floor(this Vector3 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
                
            return new Vector3((float)Math.Floor(v.x*pow)/pow, (float)Math.Floor(v.y*pow)/pow,(float)Math.Floor(v.z*pow)/pow);
        }
        public static Vector2 Ceiling(this Vector2 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
            return new Vector2((float)Math.Ceiling(v.x*pow)/pow, (float)Math.Ceiling(v.y*pow)/pow);
        }
        public static Vector3 Ceiling(this Vector3 v, int decimalPlaces = 0)
        {
            var pow = (float)Math.Pow(10, decimalPlaces);
                
            return new Vector3((float)Math.Ceiling(v.x*pow)/pow, (float)Math.Ceiling(v.y*pow)/pow,(float)Math.Ceiling(v.z*pow)/pow);
        }

        public static float Dot(this Vector3 v, Vector3 u)
        {
            return Vector3.Dot(v,u);
        }
        public static float Dot(this Vector2 v, Vector2 u)
        {
            return Vector2.Dot(v,u);
        }
        public static bool EqualsApprox(this Vector2 v, Vector2 u, float eps = 0.001f)
        {
            return v.DistanceTo(u) <= eps;
        }
        public static bool EqualsApprox(this Vector3 v, Vector3 u, float eps = 0.001f)
        {
            return v.DistanceTo(u) <= eps;
        }

        /// <summary>
        /// inverts pointwise
        /// </summary>
        /// <returns></returns>
        public static Vector3 Invert(this Vector3 v)
        {
            return new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
        }
        /// <summary>
        /// scales pointwise
        /// </summary>
        /// <returns></returns>
        public static Vector3 ScaleByVector(this Vector3 v, Vector3 w)
        {
            return new Vector3(v.x*w.x,  v.y*w.y, v.z*w.z);
        }

        /// <summary>
        /// scales pointwise
        /// </summary>
        /// <returns></returns>
        public static Vector2 Scale(this Vector2 v, float x, float y)
        {
            return new Vector2(v.x*x,  v.y*y);
        }


        public static Vector2 FromPolar(float r, float thetaInRadians)
        {
            return new Vector2((float)Math.Cos(thetaInRadians) * r, (float)Math.Sin(thetaInRadians) * r);
        }
        public static Vector2 FromPolarDegrees(float r, float theta)
        {
            return FromPolar(r, theta * (float)Math.PI / 180f);
        }
        
        
        public static (float, Vector3) ProjectToLine(this Vector3 v, Vector3 point, Vector3 direction)
        {
            //gets the projection of v onto the line defined by point and direction
            //https://math.stackexchange.com/questions/175896/finding-a-point-along-a-line-a-certain-distance-away-from-another-point
            var t = Vector3.Dot(v - point, direction) / Vector3.Dot(direction, direction);
            var proj = point + t * direction;
            Debug.DrawLine(point, proj, Color.red);
            return (t, proj);
        }
    }
}