using System;
using System.Drawing;
using System.Drawing.Text;
namespace Senses
{
    class Theme
    {
        private Color background;
        private Color foreground;
        private Font font;
        PrivateFontCollection fontCollection;
        public Theme(Color background, Color foreground, Font font)
        {
            Build(background, foreground, font);
        }
        public Theme()
        {
            Color background = new Color(0, 0, 0);
            Color foreground = new Color(255, 255, 255);
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("fonts/JetBrainsMono-1.0.3/ttf/JetBrainsMono-ExtraBold.ttf");
            Build(background, foreground, new Font(fontCollection.Families[0], 16));
        }
        private void Build(Color background, Color foreground, Font font)
        {
            this.background = background;
            this.foreground = foreground;
            this.font = font;
        }
        public Font Font
        {
            get{ return font; }
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
}
