using System;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Common
{
    public class Bobber : MonoBehaviour
    {
    
        public float bobAmplitudeX = 0; //degrees per second
        public float bobAmplitudeY = 1; //degrees per second
        public float bobAmplitudeZ = 0; //degrees per second

        public float bobFrequency = 1;
        private Vector3 _basePosition;

        void Start()
        {
            _basePosition = transform.position;
        }

        void Update()
        {
            var offset = new Vector3(bobAmplitudeX, bobAmplitudeY, bobAmplitudeZ) *
                         (float)Math.Cos(Time.timeSinceLevelLoad * Math.PI * 2* bobFrequency);
            transform.position = _basePosition + offset;
        }
    }
}
