using GatewayController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCli
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create gateway object
            Gateway gateway = new Gateway();

            // Start the gateway, it will take some time
            gateway.Start("963", "", "", false);

            // Do some work here
            // ....

            // Stop the gateway now. I hope you have telnet feature turned on
            gateway.Stop();
        }
    }
}
