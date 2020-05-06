namespace Learn
{
    class Math
    {
        public static long Distance(long a, long b)
        {
            long big, small;
            if (a < b)
            {
                big = b;
                small = a;
            }
            else
            {
                big = a;
                small = b;
            }
            return big - small;
        }
    }
}
