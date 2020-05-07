using System;
using System.Drawing;
namespace Senses
{
    class TextVisual : Visual
    {
        private bool shouldUpDateTextImage;
        private Bitmap textImage;
        private string text;
        internal string Text
        {
            set 
            {
                if (!text.Equals(value))
                {
                    text = value;
                    shouldUpDateTextImage = true;
                    base.Window.RePaint();
                }
            }
        }
        public TextVisual(string text) : base(0, 0, 1, 1)
        {
            this.text = text;
            UpDateTextImage();
        }
        private void UpDateTextImage()
        {
            if (Theme == null)
            {
                Theme = Window.Theme;
            }
            Color foreground = Theme.Foreground;
            System.Drawing.Color systemForeground = System.Drawing.Color.FromArgb(foreground.Red, foreground.Green, foreground.Blue);
            System.Drawing.Color systemBackground = System.Drawing.Color.FromArgb(0,0,0,0);
            textImage = DrawText(text, Window.Theme.Font, systemForeground, systemBackground);
            shouldUpDateTextImage = false;
        }
        internal override void Draw(PixelDrawer pixelDrawer)
        {
            base.Draw(pixelDrawer);
            if (shouldUpDateTextImage)
            {
                UpDateTextImage();
            }
            int x0 = Position.X + Size.Width / 2 - textImage.Width / 2;
            int y0 = Position.Y + Size.Height / 2 - textImage.Height / 2; 
            System.Drawing.Color systemColor;
            Color color;
            for (int y = 0; y < textImage.Height; y++)
            {
                for (int x = 0; x < textImage.Width; x++)
                {
                    //byte grey =
                    systemColor = textImage.GetPixel(x,y);
                    if (systemColor.A > 0)
                    {
                        color = new Color(systemColor.R, systemColor.G, systemColor.B);
                        pixelDrawer.DrawPixel(x0 + x, y0 + y, color);
                    }
                    //Console.WriteLine("pixel {0}, {1} = {2}", x, y, textImage.GetPixel(x,y));
                } 
            }

        }
        private Bitmap DrawText(String text, Font font, System.Drawing.Color textColor, System.Drawing.Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Bitmap img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int) textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);
            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }
    }
}
