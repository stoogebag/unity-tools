using UnityEngine;

namespace stoogebag.DebugTools
{
    public class FPSInCorner : TextInCorner
    {
        public int cacheSize = 5;
        public float waitTime = 5;

        private int index = 0;
        private float[] _cache;
        private bool _waitElapsed;
        private float _lowestSinceWait;

        private void Awake()
        {
            _cache = new float[cacheSize];
        }

        public override string GetText()
        {
            var val = 1f / Time.unscaledDeltaTime;

            if (! _waitElapsed && Time.time >= waitTime)
            {
                _waitElapsed = true;
                _lowestSinceWait = val;
            }

            if (_waitElapsed)
            {
                _lowestSinceWait = Mathf.Min(_lowestSinceWait, val);
                var num = _lowestSinceWait;
                _cache[index] = val;
                index = (index + 1) % cacheSize;
                return $"{val:F0} FPS (lowest: {num:F0})";
            }

            _cache[index] = val;
            index = (index + 1) % cacheSize;
            return $"{val:F0} FPS";
        }
    }
}