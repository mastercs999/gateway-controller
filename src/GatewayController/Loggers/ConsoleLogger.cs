using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayController.Loggers
{
    public class ConsoleLogger : Logger, ILogger
    {
        public new void Write(params string[] lines)
        {
            Console.Write(base.Write(lines));
        }

        public new void Write(Exception ex)
        {
            Console.Write(base.Write(ex));
        }
    }
}
