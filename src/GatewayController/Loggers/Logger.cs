using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GatewayController.Extensions;

namespace GatewayController.Loggers
{
    public abstract class Logger
    {
        public string LastMessage { get; private set; }

        protected string Timestamp => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff");

        private string ThisNamespace = typeof(Logger).Namespace;



        protected string Write(params string[] lines)
        {
            LastMessage = lines?.FirstOrDefault();

            return FormatOutput(lines);
        }

        protected string Write(Exception ex)
        {
            LastMessage = ex?.Message;

            List<string> lines = new List<string>();

            while (ex != null)
            {
                lines.AddRange(new string[]
                {
                    "<EXCEPTION>",
                    ex.Message,
                    "\n",
                    ex.StackTrace,
                    "\n",
                    ex.ToString(),
                    "\n",
                    "<INNER EXCEPTION> " + (ex.InnerException == null ? "<NULL>" : "<FOLLOWS>")
                });

                ex = ex.InnerException;
            }

            return FormatOutput(lines.ToArray());
        }




        private string FormatOutput(string[] lines)
        {
            // Make sure there are no new lines between
            lines = String.Join("\n", lines).Split(new string[] { "\n" }, StringSplitOptions.None);

            // Get timestamp
            string timestamp = Timestamp;

            // Get caller information
            List<Caller> callers = FindCallers();

            // Append header
            string header = timestamp + " " + String.Join(" <= ", callers.Select(x => $"{x.MethodName} ({x.ClassName})"));
            for (int i = 0; i < lines.Length; ++i)
                lines[i] = new string(' ', timestamp.Length + 1) + lines[i];

            // Return as a string
            return header + "\n" + String.Join("\n", lines) + "\n";
        }

        private List<Caller> FindCallers()
        {
            StackTrace stack = new StackTrace();
            List<Caller> callers = new List<Caller>(stack.FrameCount);

            for (int i = 0; i < stack.FrameCount; ++i)
            {
                // Find calling type
                MethodBase callingMethod = stack.GetFrame(i)?.GetMethod();
                Type callingType = callingMethod?.DeclaringType;
                if (callingMethod == null || callingType == null || callingType.Namespace == ThisNamespace)
                    continue;

                callers.Add(new Caller()
                {
                    ClassName = callingType.GetRealTypeName(),
                    MethodName = callingMethod.Name
                });
            }

            return callers;
        }

        private class Caller
        {
            public string ClassName { get; set; }
            public string MethodName { get; set; }
        }
    }
}
