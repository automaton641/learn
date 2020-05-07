namespace Senses
{
    class Bar : Visual
    {
        double level;
        public Bar(double level)
        {
            this.level = level;
        }
        public double Level
        {
            set 
            {
                level = value;
                Window.RePaint();
            }
        }
        internal override void Draw(PixelDrawer pixelDrawer)
        {
            base.Draw(pixelDrawer);
            int x = Position.X + Size.Width / 4;
            int width = Size.Width / 2;
            int y = Position.Y;
            int height = (int)(level*Size.Height);
            pixelDrawer.DrawRectangle(new Position(x, y), new Size(width, height), Theme.Foreground);
        }
    }
}
