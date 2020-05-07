using System.Collections.Generic;
namespace Senses
{
    class Container : Visual
    {
        private double growAdder;
        private Orientation orientation;
        List<Visual> visuals;
        public List<Visual> Visuals
        {
            get {return visuals;}
        }
        public Container(int x, int y, int width, int height, Orientation orientation) : base(x, y, width, height)
        {
            Build(orientation);
        }
        public Container(Orientation orientation) : base(0, 0, 1, 1)
        {
            Build(orientation);
        }
        private void Build(Orientation orientation)
        {
            this.orientation = orientation;
            visuals = new List<Visual>();
            growAdder = 0;
        }
        internal override void Arrange(Size size, Position position) {
            base.Arrange(size, position);
            Size previousSize = new Size(0, 0);
            Position newPosition = new Position(position);
            foreach (Visual visual in visuals) {
                visual.Proportion =  visual.GrowRatio / growAdder;
                int offset;
                if (orientation == Orientation.Horizontal)
                {
                    offset = previousSize.Width;
                }
                else
                {
                    offset = previousSize.Height;
                }
                newPosition = new Position(newPosition, offset, orientation);
                visual.Arrange(new Size(size, visual.Proportion, orientation), newPosition);
                previousSize = visual.Size;
            }
        }
        public void Revalidate(Size size, Position position) {
            Arrange(size, position);
            Window?.RePaint();
        }
        internal override void Draw(PixelDrawer pixelDrawer)
        {
            base.Draw(pixelDrawer);
            foreach (Visual visual in visuals)
            {
                visual.Draw(pixelDrawer);
            }
        }
        public void Add(TextInput input)
        {
            Window.AddTextEnteredHandler(input.OnTextEntered);
            Add((Visual)input);
        }
        public void Add(Button button)
        {
            Window.AddMouseButtonPressedHandler(button.onMouseButtonPressed);
            Add((Visual)button);
        }
        public void Add(Visual visual)
        {
            growAdder += visual.GrowRatio;
            visual.Window = Window;
            visuals.Add(visual);
            Revalidate(Size, Position);
        }
    }
}
