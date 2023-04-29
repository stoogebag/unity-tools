using System;
using TMPro;
using UnityEngine;

namespace stoogebag.Common
{
    public class TimerText : MonoBehaviour
    {
        public string FormatString = "mm':'ss";
        private TextMeshProUGUI text;

        private DateTime _startTime;

        private event Action Finished;

        public bool StartOnAwake = false;
        public bool CountDown = false;
        public float CountDownTimeInSeconds = 300f;

        private bool _running; 
    
        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();


            if (StartOnAwake)
            {
                if(CountDown) StartCountdown(TimeSpan.FromSeconds(CountDownTimeInSeconds));
                else StartTimer();
            }
        
        }

        void Update()
        {
            if (_running)
            {
                var timeSinceStart = DateTime.Now - _startTime;

                var time = CountDown ? (TimeSpan.FromSeconds(CountDownTimeInSeconds) - timeSinceStart) : timeSinceStart;

                if (time <= TimeSpan.Zero)
                {
                    if (CountDown)
                    {
                        _running = false;
                        Finished?.Invoke();
                        time = TimeSpan.Zero;
                    }
                }
            
                var s = time.ToString(FormatString);

                text.text = s;
            }
        }

        public void StartTimer()
        {
            _startTime = DateTime.Now;
            CountDown = false;
            _running = true;
        }

        public void StartCountdown(TimeSpan time)
        {
            CountDown = true;
            _startTime = DateTime.Now;
            CountDownTimeInSeconds = (float)time.TotalSeconds;
            _running = true;
        }

        public void Reset()
        {
            _startTime = DateTime.MinValue;
            _running = false;
        }

    }
}
