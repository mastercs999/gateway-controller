using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayController.Loggers
{
    public class FileLogger : Logger, ILogger
    {
        private readonly string DirectoryPath;
        private object Lock = new object();




        public FileLogger(string logDirectory)
        {
            DirectoryPath = logDirectory;

            // Create directory if doesn't exist
            Directory.CreateDirectory(DirectoryPath);
        }




        public new void Write(params string[] lines)
        {
            Flush(base.Write(lines));
        }

        public new void Write(Exception ex)
        {
            Flush(base.Write(ex));
        }

        private void Flush(string content)
        {
            // Write to the file
            lock (Lock)
                File.AppendAllText(Path.Combine(DirectoryPath, DateTimeOffset.UtcNow.ToString("yyyy_MM_dd") + ".log"), content);
        }
    }
}
