using System;
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

        public static Vector2 Rounded(this Vector2 v)
        {
            return new Vector2((float)Math.Round(v.x), (float)Math.Round(v.y));
        }
        public static Vector3 Rounded(this Vector3 v)
        {
            return new Vector3((float)Math.Round(v.x), (float)Math.Round(v.y),(float)Math.Round(v.z));
        }

        public static float Dot(this Vector3 v, Vector3 u)
        {
            return Vector3.Dot(v,u);
        }
        public static float Dot(this Vector2 v, Vector2 u)
        {
            return Vector2.Dot(v,u);
        }

    }
}