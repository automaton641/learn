using System;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
namespace Senses
{
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
        public Container Container
        {
            get {return container;}
        }
        public Window(string title, int width, int height, Orientation orientation)
        {
            Build(title, width, height, orientation);
        }
        public Window(string title, int width, int height)
        {
            Build(title, width, height, Orientation.Horizontal);
        }
        private void Build(string title, int width, int height, Orientation orientation)
        {
            theme = new Theme();
            this.title = title;
            this.size = new Size(width, height);
            window = new RenderWindow(new VideoMode((uint)width, (uint)height), title, Styles.Close);
            window.Closed += new EventHandler(OnClosed);
            VideoMode desktop = VideoMode.DesktopMode;
            window.Position = new Vector2i((int)desktop.Width / 2 - (int)window.Size.X / 2, (int)desktop.Height / 2 - (int)window.Size.Y / 2);
            container = new Container(0, 0, width, height, orientation);
            container.Window = this;
            texture = new Texture((uint)width, (uint)height);
            pixels = new byte[width * height * 4];
            pixelDrawer = new PixelDrawer(pixels, width, height);
            sprite = new Sprite(texture);
        }
        internal void AddTextEnteredHandler(EventHandler<TextEventArgs> onTextEntered)
        {
            window.TextEntered += onTextEntered;
        }
        internal void AddMouseButtonPressedHandler(EventHandler<MouseButtonEventArgs> onMouseButtonPressed)
        {
            window.MouseButtonPressed += onMouseButtonPressed;
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
        public void RePaint()
        {
            shouldDraw = true;
        }
        private void OnClosed(object sender, EventArgs e)
        {
                window.Close();
        }

    }
}
