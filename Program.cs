using System;

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
                VisualClient visualClient = new VisualClient();
            }
        }
        
    }
}
