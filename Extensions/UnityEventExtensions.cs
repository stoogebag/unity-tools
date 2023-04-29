using UnityEngine;
using UnityEngine.Events;

namespace stoogebag.Extensions
{
    public static class UnityEventExtensions
    {

    }

    [System.Serializable]
    public class UnityEventVector3 : UnityEvent<Vector3>
    {
    }

    [System.Serializable]
    public class UnityEventArrayVector3 : UnityEvent<Vector3[]>
    {
    }

    [System.Serializable]
    public class UnityEventFuncArrayVector3 : UnityEventFunc<Vector3[]>
    {
    }

    [System.Serializable]
    public class UnityEventFunc<T> : UnityEvent<T, T>
    {
    }
}