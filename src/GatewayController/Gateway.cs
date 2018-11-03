using GatewayController.Exceptions;
using GatewayController.Loggers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayController
{
    public class Gateway
    {
        [DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        private static extern bool EnableWow64FSRedirection(bool enable);

        private ILogger _Logger;
        private readonly string IBControllerDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "IBController");
        private string StartBatchPath => Path.Combine(IBControllerDirectory, "IBControllerGatewayStart.bat");
        private string StopBatchPath => Path.Combine(IBControllerDirectory, "IBControllerStop.bat");
        private string IniFile => Path.Combine(IBControllerDirectory, "IBController.ini");
        private string LogDirectory => Path.Combine(IBControllerDirectory, "Logs");




        /// <summary>
        ///  Gateway with no logging
        /// </summary>
        public Gateway() : this(new SilentLogger())
        {

        }

        /// <summary>
        /// Gateway with file or console logging
        /// </summary>
        /// <param name="logger"></param>
        public Gateway(ILogger logger)
        {
            _Logger = logger;
        }



        /// <summary>
        /// Starts IB gateway. After invoking its starts it waits 60 seconds and hopes that it was successfull.
        /// </summary>
        /// <param name="version">Major version of installed gateway. For example: 963</param>
        /// <param name="username">Username for login into gateway</param>
        /// <param name="password">Password for login into gateway</param>
        /// <param name="liveAccount">True if live account should be selected in login dialog</param>
        /// <exception cref="GatewayController.Exceptions.GatewayException">Throw when starting the gateway start fails - not able to find gateway process</exception>
        public void Start(string version, string username, string password, bool liveAccount)
        {
            Start(version, username, password, liveAccount, 60);
        }

        /// <summary>
        /// Starts IB gateway. After invoking its start it waits 60 seconds and hopes that it was successfull.
        /// </summary>
        /// <param name="version">Major version of installed gateway. For example: 963</param>
        /// <param name="username">Username for login into gateway</param>
        /// <param name="password">Password for login into gateway</param>
        /// <param name="liveAccount">True if live account should be selected in login dialog</param>
        /// <param name="afterStartTimeout">Time in seconds which should the method call wait after invoking gateway start.
        /// It is set to make sure that when this call ends the gateway is ready.</param>
        /// <exception cref="GatewayController.Exceptions.GatewayException">Throw when starting the gateway start fails - not able to find gateway process</exception>
        public void Start(string version, string username, string password, bool liveAccount, int afterStartTimeout)
        {
            _Logger.Write($"Request for starting gateway with version {version} for {username} and live account was set to {liveAccount}");

            // Prevent multiple instances
            if (IsGatewayRunning())
            {
                _Logger.Write("Gateway is already running. So we'll kill it first");
                Stop();
            }

            // Modify batch file with given information
            _Logger.Write($"Let's modify some variables in {StartBatchPath}");
            ChangeBatchVariables(StartBatchPath, new NameValueCollection()
            {
                { "TWS_MAJOR_VRSN", version },
                { "IBC_INI", IniFile },
                { "IBC_PATH", IBControllerDirectory },
                { "LOG_PATH", LogDirectory },
                { "TWSUSERID", username },
                { "TWSPASSWORD", password },
                { "TRADING_MODE", liveAccount ? "live" : "paper" }
            });
            _Logger.Write("Variables were successfully changed");

            // Start IB
            _Logger.Write($"Starting gateway with by batch file {StartBatchPath}");
            RunBatch(StartBatchPath, "/INLINE");
            _Logger.Write("Batch file was started");

            // End timeout
            Thread.Sleep(afterStartTimeout * 1000);

            // Check if it running
            if (!IsGatewayRunning())
                throw new GatewayException("Something weird has happened. Even though we started IB Gateway, we were unable to find its process");
        }

        /// <summary>
        /// This function stops the gateway. At first it tries by using IB Controller and telnet. If it fails it kills its process.
        /// </summary>
        /// <exception cref="GatewayController.Exceptions.GatewayException">Throw when stopping the gateway fails - gateway process is still running</exception>
        public void Stop()
        {
            _Logger.Write($"Request for stopping IB Gateway was received");

            // Nothing is needed to be ended
            if (!IsGatewayRunning())
            {
                _Logger.Write("IB Gateway doesn't run, so no action is needed to be taken");
                return;
            }

            // Try to end it gracefully
            _Logger.Write($"Let's run stopping batch file: {StopBatchPath}");
            RunBatch(StopBatchPath);
            Thread.Sleep(5000);
            _Logger.Write("Stopping batch file was run");

            // Kill all telnets we find - this can be remains from previous step
            _Logger.Write("Killing all telnet processes...");
            foreach (Process telnet in FindTelnetProcesses())
                telnet.Kill();
            Thread.Sleep(1000);
            _Logger.Write("All telnet processes should be killed");

            // Didn't work? Kill
            if (IsGatewayRunning())
            {
                _Logger.Write("Obviously stopping IB Gateway gracefully didn't work, so we will find its process and kill it");
                FindGatewayProcess()?.Kill();
                Thread.Sleep(5000);
                _Logger.Write("We may have killed IB Gateway process");
            }

            // Gateway shouldn't be running
            if (IsGatewayRunning())
                throw new GatewayException("Even though we tried to stop IB Gatway, we are still able to find its process");
        }




        private void RunBatch(string path)
        {
            RunBatch(path, "");
        }
        private void RunBatch(string path, string arguments)
        {
            // We need to disable redirection otherwise some things like telnet doesn't work
            EnableWow64FSRedirection(false);

            // Set up process
            ProcessStartInfo processInfo = new ProcessStartInfo(@"cmd.exe", $"/c {Path.GetFileName(path)} {arguments}")
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(path)
            };

            // Start the process
            Process.Start(processInfo);

            // Enable redirection again
            EnableWow64FSRedirection(true);
        }
        private bool IsGatewayRunning()
        {
            return FindGatewayProcess() != null;
        }
        private Process FindGatewayProcess()
        {
            // It may be run by Java
            Process gatewayProcess = Process.GetProcessesByName("java").SingleOrDefault(x => !String.IsNullOrEmpty(x.MainWindowTitle) && (x.MainWindowTitle.ToLower().Contains("ib gateway") || x.MainWindowTitle.ToLower().Contains("ibgateway")));

            // Or it runs by Java, but main window title is different
            if (gatewayProcess == null)
                using (ManagementObjectCollection results = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name = 'java.exe'").Get())
                {
                    ManagementObject managementObject = results.Cast<ManagementObject>().SingleOrDefault(x => x.Properties["CommandLine"].Value is string str && str != null && str.Contains("ibgateway"));
                    if (managementObject != null)
                        gatewayProcess = Process.GetProcessById((int)(uint)managementObject["ProcessId"]);
                }

            // Or it may be run directly
            if (gatewayProcess == null)
                gatewayProcess = Process.GetProcessesByName("ibgateway").SingleOrDefault();

            _Logger.Write(gatewayProcess == null ? "We didn't find gateway process" : $"We found a gateway process called '{gatewayProcess.ProcessName}' with window title '{gatewayProcess.MainWindowTitle}' which has started at {gatewayProcess.StartTime.ToUniversalTime()}");
            return gatewayProcess;
        }
        private Process[] FindTelnetProcesses()
        {
            return Process.GetProcessesByName("telnet");
        }
        private void ChangeBatchVariables(string batchFilePath, NameValueCollection variables)
        {
            // Load file
            List<string> content = File.ReadAllLines(batchFilePath).ToList();

            // Modify variables
            foreach (string key in variables.AllKeys)
            {
                // Find line where this variable is set
                int line = content.FindIndex(x => x.ToLower().StartsWith($"set {key.ToLower()}="));

                // Replace variable value
                content[line] = content[line].Substring(0, content[line].IndexOf('=') + 1) + variables[key];
            }

            // Write the file back
            File.WriteAllLines(batchFilePath, content);
        }
    }
}
