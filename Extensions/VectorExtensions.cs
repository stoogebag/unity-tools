﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace stoogebag
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
        
    }
}