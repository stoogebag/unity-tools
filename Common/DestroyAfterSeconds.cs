using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace stoogebag.Common
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float Time = 30;
        //private DateTime started;

        async void Start()
        {
            Destroy(gameObject,Time);
        }

    }
}
