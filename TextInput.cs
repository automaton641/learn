using SFML.Window;
using System;

namespace Senses
{
    class TextInput : TextVisual
    {
        private string label;
        private string input;
        public TextInput(string label) : base(label + ": ")
        {
            this.label = label;
            this.input = "";
        }
        public string Input
        {
            get {return input;}
            set 
            {
                if (!input.Equals(value))
                {
                    input = value;
                    Text = label + ": " + input;
                }
            }
        }
        internal void OnTextEntered(object sender, TextEventArgs e)
        {
            //Console.WriteLine("text entered: {0}", e.Unicode);
            if (e.Unicode.Length == 1)
            {
                if (e.Unicode[0] == 8)
                {
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                        Text = label + ": " + input;
                    }
                    //Console.WriteLine("backspace");
                }
                else
                {
                    input += e.Unicode;
                    Text = label + ": " + input;
                }
            }
            
        }
    }
}
