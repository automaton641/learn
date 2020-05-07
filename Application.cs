namespace Senses
{
    class Application
    {
        private Window window;
        public Application()
        {

        }
        public Window Window
        {
            set {window = value;}
            get {return window;}
        }
        public void Run()
        {
            window.Loop();
        }
    }
}
