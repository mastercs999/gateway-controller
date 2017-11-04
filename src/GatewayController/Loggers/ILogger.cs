using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayController.Loggers
{
    public interface ILogger
    {
        string LastMessage { get; }

        void Write(params string[] lines);
        void Write(Exception ex);
    }
}
