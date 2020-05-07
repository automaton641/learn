namespace Senses
{
    class PixelDrawer
    {
        private Size size;
        private byte[] pixels;
        public PixelDrawer(byte[] pixels, int width, int height)
        {
            this.pixels = pixels;
            size = new Size(width, height);
        }
        public void DrawPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= size.Width || y >= size.Height)
            {
                return;
            }
            int arrayY = y * 4;
            int arrayX = x * 4;
            int index = arrayY * size.Width + arrayX;
            pixels[index] = color.Red;
            pixels[index + 1] = color.Green;
            pixels[index + 2] = color.Blue;
            pixels[index + 3] = 255;

        }
        public void DrawRectangle(Position position, Size size, Color color)
        {
            int y0 = position.Y;
            int y1 = y0 + size.Height;
            int x0 = position.X;
            int x1 = x0 + size.Width;
            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    DrawPixel(x, y, color);
                }  
            }
        }
    }
}
