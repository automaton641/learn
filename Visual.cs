namespace Senses
{
    class Visual
    {
        private Window window;
        internal Window Window
        {
            get {return window;}
            set {window = value;}
        }
        private Position position;
        private double proportion;
        private Size size;
        private double growRatio;
        private Theme theme;
        public Theme Theme
        {
            get {return theme;}
            set {theme = value;}
        }

        internal double GrowRatio
        {
            get {return growRatio;}
        }
        internal Size Size
        {
            get {return size;}
        }
        internal double Proportion
        {
            get {return proportion;}
            set {proportion = value;}
        }
        internal Position Position
        {
            get {return position;}
        }
        public Visual() 
        {
            Build(0, 0, 1, 1);
        }
        public Visual(int x, int y, int width, int height)
        {
            Build(x, y, width, height);
        }
        private void Build(int x, int y, int width, int height)
        {
            growRatio = 1.0;
            proportion = 1.0;
            position = new Position(x, y);
            size = new Size(width, height);
        }
        internal virtual void Arrange(Size size, Position position) 
        {
            this.size = size;
            this.position = position;
        }

        internal virtual void Draw(PixelDrawer pixelDrawer)
        {
            if (theme == null)
            {
                theme = Window.Theme;
            }
            pixelDrawer.DrawRectangle(position, size, theme.Background);
        }
    }
}
