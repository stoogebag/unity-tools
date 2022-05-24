using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace stoogebag
{
    public class Timer : MonoBehaviour
    {
        public float CoolDownInSeconds = 1;
        float timeSinceLast = 0f;

        public UnityEvent Event;

        private void Update()
        {
            timeSinceLast += Time.deltaTime;

            if (timeSinceLast > CoolDownInSeconds)
            {
                timeSinceLast -= CoolDownInSeconds;
                Event?.Invoke();
            }

        }

    }
}
