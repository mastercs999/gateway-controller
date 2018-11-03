using GatewayController;
using System;
using System.Linq;

namespace ExampleCli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create gateway object              
                var conLog = new GatewayController.Loggers.ConsoleLogger();
                var gw = new Gateway(conLog);

                // Start the gateway, it will take some time
                gw.Start("963", string.Empty, string.Empty, false);

                // Do some work here
                // ....

                // Stop the gateway now. I hope you have telnet feature turned on
                gw.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
