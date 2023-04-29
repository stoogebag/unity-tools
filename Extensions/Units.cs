using System;

namespace stoogebag.Extensions
{
    public static class Units {

        public static float KMHToMetresPerSecond(this float speed)
        {
            return speed / 3.6f;
        }
        public static float MetresPerSecondToKMH(this float speed)
        {
            return speed * 3.6f;
        }
        public static float ToRadians(this float angleInDegrees)
        {
            return angleInDegrees * PiOver180;
        }
        public static float ToDegrees(this float angleInRads)
        {
            return angleInRads / PiOver180;
        }

        private const float PiOver180 = (float)Math.PI/180;
    }
}