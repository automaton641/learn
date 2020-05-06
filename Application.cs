using System;
using System.Collections.Generic;
using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
namespace Senses
{
    class Size
    {
        private int width;
        private int height;
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public int Width
        {
            get{ return width; }
        }
        public int Height
        {
            get{ return height; }
        }
    }
    class Position
    {
        private int x;
        private int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
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
    class Visual
    {
        private Position position;
        private Size size;
        public Visual(int x, int y, int width, int height)
        {
            position = new Position(x, y);
            size = new Size(width, height);
        }
        internal virtual void Draw(PixelDrawer pixelDrawer)
        {
            pixelDrawer.DrawRectangle(position, size, Window.Theme.Background);
        }
    }
    enum Orientation
    {
        Horizontal,
        Vertical,
    }
    class TextVisual : Visual
    {
        string text;
        public TextVisual(string text) : base(0, 0, 1, 1)
        {
            this.text = text;
        }
        internal override void Draw(PixelDrawer pixelDrawer)
        {
            base.Draw(pixelDrawer);

        }

    }
    class Container : Visual
    {
        private Orientation orientation;
        List<Visual> visuals;
        public Container(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }
        internal override void Draw(PixelDrawer pixelDrawer)
        {
            base.Draw(pixelDrawer);
        }
        
    }
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
    class Window
    {
        private static Theme theme;
        public static Theme Theme
        {
            get{return theme;}
        }
        private Sprite sprite;
        private PixelDrawer pixelDrawer;
        private Texture texture;
        private Container container;
        private byte[] pixels;
        private RenderWindow window;
        private string title;
        private Size size;
        private bool shouldDraw = true;
        public Window(string title, int width, int height)
        {
            theme = new Theme();
            this.title = title;
            this.size = new Size(width, height);
            window = new RenderWindow(new VideoMode((uint)width, (uint)height), title, Styles.Close);
            window.Closed += new EventHandler(OnClosed);
            VideoMode desktop = VideoMode.DesktopMode;
            window.Position = new Vector2i((int)desktop.Width / 2 - (int)window.Size.X / 2, (int)desktop.Height / 2 - (int)window.Size.Y / 2);
            container = new Container(0, 0, width, height);
            texture = new Texture((uint)width, (uint)height);
            pixels = new byte[width * height * 4];
            pixelDrawer = new PixelDrawer(pixels, width, height);
            sprite = new Sprite(texture);
        }
        internal void Loop()
        {
            window.SetVisible(true);
            while (window.IsOpen)
            {
                    window.DispatchEvents();
                    if (shouldDraw)
                    {
                        //window.Clear(Color.Black);
                        container.Draw(pixelDrawer);
                        texture.Update(pixels);
                        window.Draw(sprite);
                        window.Display();
                        shouldDraw = false;
                    }
            }
        }
        private void OnClosed(object sender, EventArgs e)
        {
                window.Close();
        }

    }
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
    class Theme
    {
        private Color background;
        private Color foreground;
        public Theme()
        {
            background = new Color(0, 0, 255);
            foreground = new Color(255, 255, 255);
        }
        public Color Foreground
        {
            get{ return foreground; }
        }
        public Color Background
        {
            get{ return background; }
        }
    }
    class Application
    {
        private Window window;
        public Application()
        {

        }
        public Window Window
        {
            set {window = value;}
        }
        public void Run()
        {
            window.Loop();
        }
    }
}
