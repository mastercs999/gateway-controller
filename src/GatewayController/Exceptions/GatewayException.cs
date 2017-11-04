using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayController.Exceptions
{
    public class GatewayException : Exception
    {
        public GatewayException()
        {
        }

        public GatewayException(string message) : base(message)
        {
        }

        public GatewayException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
