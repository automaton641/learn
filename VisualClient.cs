using System;
using Senses;

namespace Learn
{
    class VisualClient
    {
        private Application application;
        private Window window;
        private TextVisual text;
        private Theme themeA;
        public void OnConnectClicked(object sender, EventArgs e)
        {
            text.Text = "Connecting...";
            Console.WriteLine("OnConnectClicked");
        }
        public VisualClient()
        {
            application = new Application();
            window = new Window("learn", 1280, 720, Orientation.Vertical);
            themeA = new Theme(new Color(255/8, 255/8, 255 /8), new Color(255, 255, 255), Window.Theme.Font);
            application.Window = window;
            text = new TextVisual("Waiting...");
            TextInput input = new TextInput("Server");
            Button button = new Button("Connect");
            button.Theme = themeA;
            button.OnPressed += OnConnectClicked;
            window.Container.Add(text);
            window.Container.Add(input);
            window.Container.Add(button);
            application.Run();
        }
    }
}
