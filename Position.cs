namespace Senses
{
    class Position
    {
        private int x;
        private int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Position(Position position)
        {
            this.x = position.X;
            this.y = position.Y;
        }
        public Position(Position position, int offset, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                this.x = position.X + offset;
                this.y = position.Y;
            }
            else
            {
                this.x = position.X;
                this.y = position.Y + offset;
            }
        }
        public int X
        {
            get{ return x; }
        }
        public int Y
        {
            get{ return y; }
        }
    }
}
