using UnityEngine;
using UnityEngine.Events;

namespace stoogebag_MonuMental.stoogebag.Common
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
