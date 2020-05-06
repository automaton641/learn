using System;
using System.IO;
using System.Text;

namespace Learn
{
    class Program
    {
        static void Main(string[] arguments)
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
    }
}
