using System;
using SFML.Window;

namespace Senses
{
    class Button : TextVisual
    {
        public event EventHandler OnPressed;
        internal void onMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.X >= Position.X && e.Y >= Position.Y)
            {
                int xLimit = Position.X + Size.Width;
                int yLimit = Position.Y + Size.Height;
                if (e.X < xLimit && e.Y < yLimit)
                {
                    OnPressed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public Button(string text) : base(text)
        {
            
        }
    }
}
