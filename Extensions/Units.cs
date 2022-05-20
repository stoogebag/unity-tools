namespace stoogebag
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


    }
}