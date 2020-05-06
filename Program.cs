using System;
using Senses;

namespace Learn
{
    class Program
    {
        static void Main(string[] arguments)
        {
            if (arguments.Length > 0)
            {
               if (arguments[0].Equals("server", StringComparison.OrdinalIgnoreCase))
                {
                    Server server = new Server();
                }
                else
                {
                    Client client = new Client(arguments[0]);
                } 
            }
            else
            {
                Application application = new Application();
                application.Window = new Window("learn", 1280, 720);
                application.Run();
            }
        }
        
    }
}
