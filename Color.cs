namespace Senses
{
    class Color
    {
        private byte red;
        private byte green;
        private byte blue;
        public Color(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
        public byte Red
        {
            get{ return red; }
        }
        public byte Green
        {
            get{ return green; }
        }
        public byte Blue
        {
            get{ return blue; }
        }
        
    }
}
