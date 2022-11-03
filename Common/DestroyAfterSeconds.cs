using System;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Common
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        public float Time = 30;
        private DateTime started;

        void Start()
        {
            //started = DateTime.UtcNow;
            Destroy(gameObject, Time);
        }

        void Update()
        {
        }
    }
}
