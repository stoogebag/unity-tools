using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using stoogebag.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public partial class PuzzGrid : MonoBehaviour
{



    private Func<float, Vector3> PositionFuncSimple(Vector3 start, Vector3 end)
    {
        return t => start + t * (end - start);
    }

    private Func<float, Vector3> PositionFuncBoundary(Vector3 start, Vector3 end, Vector3 direction)
    {
        return t =>
        {
            if (t <= .5f) return start + t * direction;
            else return end - (1 - t) * direction;
        };
    }





}