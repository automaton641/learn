namespace Learn
{
    class Player
    {
        public const int AttributesCount = 4;
        public const long AttributesLimit = 1024;
        private long[] attributes = new long[AttributesCount];
        public Player(int index)
        {
            if (index == 0)
            {
                attributes[0] = 32;
                attributes[1] = 32;
                attributes[2] = 0;
                attributes[3] = 0;
            }
            else
            {
                attributes[0] = 32+16;
                attributes[1] = 32;
                attributes[2] = 0;
                attributes[3] = 0;
            }
        }
        public long GetAttribute(int index)
        {
            return attributes[index];
        }

        public void ChangeAttribute(int index, long offset)
        {
            attributes[index] += offset;
            if (attributes[index] > AttributesLimit)
            {
                attributes[index] = AttributesLimit;
            }
            else if (attributes[index] < 0)
            {
                attributes[index] = 0;
            }
        }
    }
}
