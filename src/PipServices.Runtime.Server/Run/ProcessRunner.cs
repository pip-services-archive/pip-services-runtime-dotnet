using System;
using System.Threading;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Run
{
    public class ProcessRunner
    {
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);
        private readonly Microservice _microservice;

        public ProcessRunner(Microservice microservice)
        {
            _microservice = microservice;
        }

        public void SetConfig(MicroserviceConfig config)
        {
            _microservice.Config = config;
        }

        public void LoadConfig(string configPath)
        {
            _microservice.LoadConfig(configPath);
        }

        public void LoadConfigWithDefault(string[] args, string defaultConfigPath)
        {
            var configPath = args.Length > 0 ? args[0] : defaultConfigPath;
            _microservice.LoadConfig(configPath);
        }

        private void CaptureErrors()
        {
            AppDomain.CurrentDomain.UnhandledException += (obj, e) =>
            {
                _microservice.Fatal(e.ExceptionObject);
                _microservice.Info("Process is terminated");

                _exitEvent.Set();
            };
        }

        private void CaptureExit()
        {
            _microservice.Info("Press Control-C to stop the microservice...");

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                _microservice.Info("Goodbye!");

                eventArgs.Cancel = true;
                _exitEvent.Set();

                Environment.Exit(1);
            };

            // Wait and close
            _exitEvent.WaitOne();
        }

        public void Run()
        {
            CaptureErrors();
            _microservice.Start();
            CaptureExit();
            _microservice.Stop();
        }

        public void RunWithConfig(MicroserviceConfig config)
        {
            SetConfig(config);
            Run();
        }

        public void RunWithConfigFile(string configPath)
        {
            LoadConfig(configPath);
            Run();
        }

        public void RunWithDefaultConfigFile(string[] args, string defaultConfigPath)
        {
            LoadConfigWithDefault(args, defaultConfigPath);
            Run();
        }
    }
}