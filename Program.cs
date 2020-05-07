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
                else if (arguments[0].Equals("client", StringComparison.OrdinalIgnoreCase))
                {
                    Client client = new Client(arguments[1]);
                }
                else
                {
                    AIClient aIClient = new AIClient(arguments[0], arguments[1]);
                } 
            }
            else
            {
                VisualClient visualClient = new VisualClient();
            }
        }
        
    }
}
