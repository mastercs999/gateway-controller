using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayController.Loggers
{
    public class SilentLogger : Logger, ILogger
    {
        public new void Write(params string[] lines)
        {
            ;
        }

        public new void Write(Exception ex)
        {
            ;
        }
    }
}
